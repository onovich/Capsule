using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using MortiseFrame.Capsule;

namespace MortiseFrame.Capsule.Tests {

    [TestFixture]
    public class SaveCoreTests {

        string testRoot;
        SaveCore core;

        [SetUp]
        public void SetUp() {
            testRoot = Path.Combine(Path.GetTempPath(), "CapsuleTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(testRoot);
            core = new SaveCore(bufferLength: 256, path: testRoot);
        }

        [TearDown]
        public void TearDown() {
            core.Clear();
            if (Directory.Exists(testRoot)) {
                Directory.Delete(testRoot, recursive: true);
            }
        }

        // ── 同步测试 ──────────────────────────────────────────────

        [Test]
        public void Sync_Save_And_Load_RoundTrip() {
            var key = core.Register(typeof(MockSave), "mock.bin");
            var original = new MockSave { IntValue = 42, FloatValue = 3.14f };
            core.Save(original, key);

            var loaded = core.TryLoad(key, out ISave result);
            Assert.IsTrue(loaded);
            var mock = result as MockSave;
            Assert.IsNotNull(mock);
            Assert.AreEqual(42, mock.IntValue);
            Assert.AreEqual(3.14f, mock.FloatValue, delta: 0.0001f);
        }

        [Test]
        public void Sync_TryLoad_Returns_False_When_File_Missing() {
            var key = core.Register(typeof(MockSave), "missing.bin");
            var loaded = core.TryLoad(key, out ISave result);
            Assert.IsFalse(loaded);
            Assert.IsNull(result);
        }

        [Test]
        public void Sync_Save_Throws_On_Buffer_Overflow() {
            var key = core.Register(typeof(OversizeMockSave), "oversize.bin");
            var oversize = new OversizeMockSave(512);
            Assert.Throws<InvalidOperationException>(() => core.Save(oversize, key));
        }

        // ── 异步测试 ──────────────────────────────────────────────

        [Test]
        public async Task Async_SaveAsync_And_TryLoadAsync_RoundTrip() {
            var key = core.Register(typeof(MockSave), "async_mock.bin");
            var original = new MockSave { IntValue = 99, FloatValue = 2.71f };
            await core.SaveAsync(original, key);

            var (success, save) = await core.TryLoadAsync(key);
            Assert.IsTrue(success);
            var mock = save as MockSave;
            Assert.IsNotNull(mock);
            Assert.AreEqual(99, mock.IntValue);
            Assert.AreEqual(2.71f, mock.FloatValue, delta: 0.0001f);
        }

        [Test]
        public async Task Async_TryLoadAsync_Returns_False_When_File_Missing() {
            var key = core.Register(typeof(MockSave), "async_missing.bin");
            var (success, save) = await core.TryLoadAsync(key);
            Assert.IsFalse(success);
            Assert.IsNull(save);
        }

        [Test]
        public async Task Async_SaveAsync_Throws_On_Buffer_Overflow() {
            var key = core.Register(typeof(OversizeMockSave), "async_oversize.bin");
            var oversize = new OversizeMockSave(512);
            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await core.SaveAsync(oversize, key));
        }

        [Test]
        public async Task Async_ConcurrentWrites_SameKey_AreSerialised() {
            var key = core.Register(typeof(MockSave), "concurrent_same.bin");
            var tasks = new Task[10];
            for (int i = 0; i < 10; i++) {
                int captured = i;
                tasks[i] = core.SaveAsync(new MockSave { IntValue = captured, FloatValue = captured }, key);
            }
            await Task.WhenAll(tasks);

            var (success, save) = await core.TryLoadAsync(key);
            Assert.IsTrue(success);
            Assert.IsNotNull(save as MockSave);
        }

        [Test]
        public async Task Async_ConcurrentWrites_DifferentKeys_RunConcurrently() {
            var key1 = core.Register(typeof(MockSave), "concurrent_key1.bin", 0);
            var key2 = core.Register(typeof(MockSave), "concurrent_key2.bin", 1);

            await Task.WhenAll(
                core.SaveAsync(new MockSave { IntValue = 1, FloatValue = 1.0f }, key1),
                core.SaveAsync(new MockSave { IntValue = 2, FloatValue = 2.0f }, key2)
            );

            var (ok1, r1) = await core.TryLoadAsync(key1);
            var (ok2, r2) = await core.TryLoadAsync(key2);
            Assert.IsTrue(ok1);
            Assert.IsTrue(ok2);
            Assert.AreEqual(1, ((MockSave)r1).IntValue);
            Assert.AreEqual(2, ((MockSave)r2).IntValue);
        }

        [Test]
        public void Async_SaveAsync_Throws_On_Cancelled_Token() {
            var key = core.Register(typeof(MockSave), "cancel_save.bin");
            var cts = new CancellationTokenSource();
            cts.Cancel();
            Assert.ThrowsAsync<TaskCanceledException>(
                async () => await core.SaveAsync(new MockSave { IntValue = 1 }, key, cts.Token));
        }

        [Test]
        public async Task Async_TryLoadAsync_Throws_On_Cancelled_Token() {
            var key = core.Register(typeof(MockSave), "cancel_load.bin");
            await core.SaveAsync(new MockSave { IntValue = 1 }, key);

            var cts = new CancellationTokenSource();
            cts.Cancel();
            Assert.ThrowsAsync<TaskCanceledException>(
                async () => await core.TryLoadAsync(key, cts.Token));
        }

        // ── 版本控制测试 ──────────────────────────────────────────

        [Test]
        public void Version_Match_Returns_True() {
            var core1 = new SaveCore(bufferLength: 256, path: testRoot, version: 1);
            var key = core1.Register(typeof(MockSave), "version_match.bin");
            core1.Save(new MockSave { IntValue = 7 }, key);

            var core2 = new SaveCore(bufferLength: 256, path: testRoot, version: 1);
            var key2 = core2.Register(typeof(MockSave), "version_match.bin");
            var loaded = core2.TryLoad(key2, out ISave result);
            Assert.IsTrue(loaded);
            Assert.AreEqual(7, ((MockSave)result).IntValue);
        }

        [Test]
        public void Version_Mismatch_Returns_False() {
            var core1 = new SaveCore(bufferLength: 256, path: testRoot, version: 1);
            var key = core1.Register(typeof(MockSave), "version_mismatch.bin");
            core1.Save(new MockSave { IntValue = 7 }, key);

            var core2 = new SaveCore(bufferLength: 256, path: testRoot, version: 2);
            var key2 = core2.Register(typeof(MockSave), "version_mismatch.bin");
            var loaded = core2.TryLoad(key2, out ISave result);
            Assert.IsFalse(loaded);
            Assert.IsNull(result);
        }

        // ── IDService ushort 测试 ──────────────────────────────────

        [Test]
        public void IDService_ushort_Supports_More_Than_255_Keys() {
            var testCore = new SaveCore(bufferLength: 256, path: testRoot);
            for (int i = 0; i < 300; i++) {
                var key = testCore.Register(typeof(MockSave), $"key_{i}.bin", index: i);
                Assert.IsTrue(key > 0, $"Key {i} should be positive");
            }
            // 验证第 256 个 key 不会溢出归零
            var key256 = testCore.Register(typeof(MockSave), "key_300.bin", index: 300);
            Assert.IsTrue(key256 > 255, "Key 256 should exceed byte range");
        }

        // ── CRC32 测试 ────────────────────────────────────────────

        [Test]
        public void Crc32_Valid_Data_Loads_Successfully() {
            var crcCore = new SaveCore(bufferLength: 256, path: testRoot, enableCrc: true);
            var key = crcCore.Register(typeof(MockSave), "crc_valid.bin");
            crcCore.Save(new MockSave { IntValue = 123, FloatValue = 4.56f }, key);

            var loaded = crcCore.TryLoad(key, out ISave result);
            Assert.IsTrue(loaded);
            Assert.AreEqual(123, ((MockSave)result).IntValue);
        }

        [Test]
        public void Crc32_Corrupted_Data_Returns_False() {
            var crcCore = new SaveCore(bufferLength: 256, path: testRoot, enableCrc: true);
            var key = crcCore.Register(typeof(MockSave), "crc_corrupt.bin");
            crcCore.Save(new MockSave { IntValue = 99 }, key);

            // 手动篡改文件第 2 字节
            var filePath = System.IO.Path.Combine(testRoot, "crc_corrupt.bin");
            var bytes = System.IO.File.ReadAllBytes(filePath);
            bytes[1] ^= 0xFF;
            System.IO.File.WriteAllBytes(filePath, bytes);

            var loaded = crcCore.TryLoad(key, out ISave result);
            Assert.IsFalse(loaded);
            Assert.IsNull(result);
        }

    }

}
