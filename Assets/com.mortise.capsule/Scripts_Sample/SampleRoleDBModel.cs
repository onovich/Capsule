using System;
using MortiseFrame.LitIO;
using UnityEngine;

namespace MortiseFrame.Capsule {

	public enum RoleType {
		Normal,
		Boss,
	}

	[Serializable]
	public struct SampleRoleDBModel : ISave {

		public int typeID;
		public string typeName;
		public RoleType roleType;
		public float atk;
		public Vector3 position;
		public Quaternion rotation;
		public bool canFly;
		public int[] skillTypeIDArr;

		public void WriteTo(byte[] dst, ref int offset) {
			ByteWriter.Write<Int32>(dst, typeID, ref offset);
			ByteWriter.WriteUTF8String(dst, typeName, ref offset);
			ByteWriter.Write<Int32>(dst, (Int32)roleType, ref offset);
			ByteWriter.Write<Single>(dst, atk, ref offset);
			ByteWriter.Write<Vector3>(dst, position, ref offset);
			ByteWriter.Write<Quaternion>(dst, rotation, ref offset);
			ByteWriter.Write<Boolean>(dst, canFly, ref offset);
			ByteWriter.WriteArray<Int32>(dst, skillTypeIDArr, ref offset);
		}

		public void FromBytes(byte[] src, ref int offset) {
			typeID = ByteReader.Read<Int32>(src, ref offset);
			typeName = ByteReader.ReadUTF8String(src, ref offset);
			roleType = (RoleType)ByteReader.Read<Int32>(src, ref offset);
			atk = ByteReader.Read<Single>(src, ref offset);
			position = ByteReader.Read<Vector3>(src, ref offset);
			rotation = ByteReader.Read<Quaternion>(src, ref offset);
			canFly = ByteReader.Read<Boolean>(src, ref offset);
			skillTypeIDArr = ByteReader.ReadArray<Int32>(src, ref offset);
		}

	}

}

