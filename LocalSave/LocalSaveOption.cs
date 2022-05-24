using System;
using MessagePack;
using UnityEngine;

#nullable enable
namespace Skyzi000.MessagePack.LocalSave
{
    /// <summary>
    /// ローカルセーブの設定
    /// </summary>
    [Serializable, MessagePackObject]
    public class LocalSaveOption : SerializableDataWithDefaultInstance<LocalSaveOption>
    {
        /// <summary>
        /// 既にファイルが存在する場合はバックアップ先に退避する
        /// </summary>
        [Key(0)]
        [field: SerializeField, Tooltip("既にファイルが存在する場合はバックアップ先に退避する")]
        public bool BackupPreviousData { get; set; } = true;

        /// <summary>
        /// 一時ファイルで置き換える前に、書き込んだデータが正しいか検証する
        /// </summary>
        [Key(1)]
        [field: SerializeField, Tooltip("一時ファイルで置き換える前に、書き込んだデータが正しいか検証する")]
        public bool VerifyBeforeReplacement { get; set; } = true;

        /// <summary>
        /// 一時ファイルで置き換えた後に、書き込んだデータが正しいか検証する
        /// </summary>
        [Key(2)]
        [field: SerializeField, Tooltip("一時ファイルで置き換えた後に、書き込んだデータが正しいか検証する")]
        public bool VerifyAfterReplacement { get; set; } = true;

        /// <summary>
        /// 保存したパスを<see cref="PlayerPrefs"/>に保存する(<see cref="LocalLoadOption.ResetBaseDirectoryOnFailure"/>に必要)
        /// </summary>
        [Key(3)]
        [field: SerializeField, Tooltip("保存したパスをPlayerPrefsに保存する(ResetBaseDirectoryOnFailureに必要)")]
        public bool SaveSavedPathToPlayerPrefs { get; set; } = false;
    }
}
