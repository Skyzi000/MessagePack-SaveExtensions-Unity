using System;
using MessagePack;
using UnityEngine;

#nullable enable

namespace Skyzi000.MessagePack.LocalSave
{
    /// <summary>
    /// ローカルロードの設定
    /// </summary>
    [Serializable, MessagePackObject]
    public class LocalLoadOption : SerializableDataWithDefaultInstance<LocalLoadOption>
    {
        /// <summary>
        /// 読み込みに失敗した場合、代わりにバックアップからの復元を試みる(一つ前のデータに巻き戻る)
        /// </summary>
        [Key(0)]
        [field: SerializeField, Tooltip("読み込みに失敗した場合、代わりにバックアップからの復元を試みる(一つ前のデータに巻き戻る)")]
        public bool RestoreFromBackupOnFailure { get; set; } = true;

        /// <summary>
        /// 読み込みに失敗した場合かつ<see cref="Local.BaseDirectoryPath"/>が<see cref="Local.DefaultBaseDirectoryPath"/>と異なる場合は、
        /// <see cref="Local.BaseDirectoryPath"/>を<see cref="Local.DefaultBaseDirectoryPath"/>に変更して読み込み直す
        /// </summary>
        [Key(1)]
        [field: SerializeField, Tooltip("読み込みに失敗した場合かつベースディレクトリが規定のディレクトリと異なる場合に、設定を規定のディレクトリに変更して読み込み直す")]
        public bool ResetBaseDirectoryOnFailure { get; set; } = true;
    }
}
