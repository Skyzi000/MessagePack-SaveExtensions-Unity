using MessagePack;

#nullable enable

namespace Skyzi000.MessagePack.LocalSave
{
    public interface ILocalSaveData : ISerializable
    {
        /// <summary>
        /// 保存時のディレクトリ名
        /// </summary>
        [IgnoreMember]
        public string DirectoryName { get; set; }

        /// <summary>
        /// 保存時のファイル名
        /// </summary>
        [IgnoreMember]
        public string FileName { get; set; }
    }
}
