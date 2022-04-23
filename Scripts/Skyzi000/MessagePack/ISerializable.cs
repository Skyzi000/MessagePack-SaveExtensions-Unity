#nullable enable

namespace Skyzi000.MessagePack
{
    /// <summary>
    /// シリアライズすることのできるデータ
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// シリアライズする
        /// </summary>
        /// <returns>シリアライズ済みのバイト配列</returns>
        public byte[] Serialize();
    }
}
