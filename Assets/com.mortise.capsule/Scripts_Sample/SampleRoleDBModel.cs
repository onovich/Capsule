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

		public int id;
		public string roleName;
		public float atk;
		public Vector3 position;
		public Vector3Int cellPos;
		public Quaternion rotation;
		public bool isDead;
		public bool[] cells;
		public string[] names;
		public RoleType roleType;

		public void WriteTo(byte[] dst, ref int offset) {
			ByteWriter.Write<Int32>(dst, id, ref offset);
			ByteWriter.WriteUTF8String(dst, roleName, ref offset);
			ByteWriter.Write<Single>(dst, atk, ref offset);
			ByteWriter.Write<Vector3>(dst, position, ref offset);
			ByteWriter.Write<Vector3Int>(dst, cellPos, ref offset);
			ByteWriter.Write<Quaternion>(dst, rotation, ref offset);
			ByteWriter.Write<Boolean>(dst, isDead, ref offset);
			ByteWriter.WriteArray<Boolean>(dst, cells, ref offset);
			ByteWriter.WriteUTF8StringArray(dst, names, ref offset);
			ByteWriter.Write<Int32>(dst, (Int32)roleType, ref offset);
		}

		public void FromBytes(byte[] src, ref int offset) {
			id = ByteReader.Read<Int32>(src, ref offset);
			roleName = ByteReader.ReadUTF8String(src, ref offset);
			atk = ByteReader.Read<Single>(src, ref offset);
			position = ByteReader.Read<Vector3>(src, ref offset);
			cellPos = ByteReader.Read<Vector3Int>(src, ref offset);
			rotation = ByteReader.Read<Quaternion>(src, ref offset);
			isDead = ByteReader.Read<Boolean>(src, ref offset);
			cells = ByteReader.ReadArray<Boolean>(src, ref offset);
			names = ByteReader.ReadUTF8StringArray(src, ref offset);
			roleType = (RoleType)ByteReader.Read<Int32>(src, ref offset);
		}

	}

}

