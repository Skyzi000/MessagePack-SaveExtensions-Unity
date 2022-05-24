using System;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;
using MessagePack;
using UnityEngine;

#nullable enable

namespace Skyzi000.MessagePack.LocalSave
{
    /// <summary>
    /// 共通メンバ
    /// </summary>
    public static partial class Local
    {
        /// <summary>
        /// 既定の<see cref="SavedBaseDirectoryPath"/>
        /// </summary>
        public static string DefaultBaseDirectoryPath => Application.persistentDataPath;

        /// <summary>
        /// ローカルセーブファイルのベースディレクトリパス(キャッシュされた上でPlayerPrefsに保存される)
        /// </summary>
        public static string SavedBaseDirectoryPath
        {
            get
            {
                if (_baseDirectoryPath != null)
                    return _baseDirectoryPath;
                _baseDirectoryPath = PlayerPrefs.GetString(BaseDirectoryPathPrefsKey, string.Empty);
                if (!string.IsNullOrEmpty(_baseDirectoryPath))
                    return _baseDirectoryPath;
                _baseDirectoryPath = DefaultBaseDirectoryPath;
                PlayerPrefs.SetString(BaseDirectoryPathPrefsKey, _baseDirectoryPath);
                PlayerPrefs.Save();

                return _baseDirectoryPath;
            }
        }

        private const string BaseDirectoryPathPrefsKey = "LocalSaveBaseDirectoryPath";
        private static string? _baseDirectoryPath = null;

        /// <summary>
        /// <see cref="PlayerPrefs"/>とキャッシュ(<see cref="_baseDirectoryPath"/>)に保存された<see cref="SavedBaseDirectoryPath"/>を削除して
        /// <see cref="DefaultBaseDirectoryPath"/>にリセットする
        /// </summary>
        /// <param name="newBaseDirectoryPath"><see cref="DefaultBaseDirectoryPath"/>の代わりに<see cref="_baseDirectoryPath"/>に設定するパス</param>
        [PublicAPI]
        public static void ResetBaseDirectoryPath(string? newBaseDirectoryPath = null)
        {
            PlayerPrefs.DeleteKey(BaseDirectoryPathPrefsKey);
            PlayerPrefs.Save();
            _baseDirectoryPath = string.IsNullOrEmpty(newBaseDirectoryPath) ? DefaultBaseDirectoryPath : newBaseDirectoryPath;
        }

        /// <summary>
        /// 一時ファイルの拡張子
        /// </summary>
        private const string TempFileExtension = ".tmp";

        /// <summary>
        /// バックアップファイルの拡張子
        /// </summary>
        private const string BackupFileExtension = ".bkup";

        /// <summary>
        /// ローカルファイルのパス
        /// </summary>
        /// <param name="baseDirectoryPath">ベースディレクトリパス</param>
        /// <param name="directoryName">ディレクトリ名</param>
        /// <param name="fileName">ファイル名</param>
        /// <returns>ファイルパス</returns>
        private static string GetFilePath(string baseDirectoryPath, string directoryName, string fileName) =>
            Path.Combine(baseDirectoryPath, directoryName.ToLower(CultureInfo.InvariantCulture), fileName.ToLower(CultureInfo.InvariantCulture));

        /// <summary>
        /// ローカルディレクトリのパス
        /// </summary>
        /// <param name="baseDirectoryPath">ベースディレクトリパス</param>
        /// <param name="directoryName">ディレクトリ名</param>
        /// <returns>ディレクトリパス</returns>
        private static string GetDirectoryPath(string baseDirectoryPath, string directoryName) =>
            Path.Combine(baseDirectoryPath, directoryName.ToLower(CultureInfo.InvariantCulture));

        /// <summary>
        /// シリアライズしてからデシリアライズすることでディープコピーを作成する
        /// </summary>
        /// <param name="original">シリアライズ可能なオブジェクト</param>
        /// <returns><see cref="original"/>のディープコピー</returns>
        public static T DeepCopy<T>(this T original) where T : ISerializable =>
            MessagePackSerializer.Deserialize<T>(MessagePackSerializer.Serialize(original));

        private static void Log(object message) => Debug.Log(message);

        private static void LogWarning(object message) => Debug.LogWarning(message);

        private static void LogError(object message) => Debug.LogError(message);

        private static void LogException(Exception exception) => Debug.LogException(exception);
    }
}
