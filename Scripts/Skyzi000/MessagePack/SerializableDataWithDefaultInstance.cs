using System;
using MessagePack;

#nullable enable

namespace Skyzi000.MessagePack
{
    public abstract class SerializableDataWithDefaultInstance<T> : ISerializable where T : class, new()
    {
        [IgnoreMember]
        protected static T? _default = null;

        [IgnoreMember]
        public static T Default
        {
            get => _default ??= new T();
            protected set => _default = value;
        }

        public virtual byte[] Serialize() => MessagePackSerializer.Serialize(this);

        /// <summary>
        /// <typeparamref name="T"/>にデシリアライズする
        /// </summary>
        public static T Deserialize(ReadOnlyMemory<byte> bytes) => MessagePackSerializer.Deserialize<T>(bytes);
    }
}
