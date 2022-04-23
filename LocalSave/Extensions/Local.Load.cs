using System;
using System.IO;
using JetBrains.Annotations;
using MessagePack;

#nullable enable

namespace Skyzi000.MessagePack.LocalSave
{
    public static partial class Local
    {
        /// <summary>
        /// ローカルファイルから読み込む
        /// </summary>
        /// <param name="data">現在のデータ(勝手に更新されない)</param>
        /// <param name="option">設定</param>
        /// <returns>読み込んでデシリアライズしたデータ、ファイルがなかったり読み込めなければ現在のデータ</returns>
        // dataを書き換えるわけではないので、戻り値を使わないと警告が出るようにPureAttributeを付けている
        [Pure, PublicAPI]
        public static T LocalLoad<T>(this T data, LocalLoadOption? option) where T : ILocalSaveData =>
            data.LocalLoad(null, null, option);

        /// <summary>
        /// <inheritdoc cref="LocalLoad{T}(T,LocalLoadOption)"/>
        /// </summary>
        /// <param name="data">現在のデータ(勝手に更新されない)</param>
        /// <param name="directoryName">読み込むディレクトリ名(指定した場合は、<see cref="data"/>に設定されている<see cref="ISerializable.DirectoryName"/>は無視される)</param>
        /// <param name="fileName">読み込むファイル名(指定した場合は、<see cref="data"/>に設定されている<see cref="ISerializable.FileName"/>は無視される)</param>
        /// <param name="option">設定</param>
        /// <returns><inheritdoc cref="LocalLoad{T}(T,LocalLoadOption)"/></returns>
        [Pure, PublicAPI]
        public static T LocalLoad<T>(this T data,
            string? directoryName = null,
            string? fileName = null,
            LocalLoadOption? option = default) where T : ILocalSaveData
        {
            directoryName ??= data.DirectoryName;
            fileName ??= data.FileName;
            return Load<T>(directoryName, fileName, option) ?? data;
        }

        // nullableではうまく解析してくれないので仕方なくJetBrains.Annotationsで凌ぐ
#nullable disable
        /// <summary>
        /// <inheritdoc cref="LocalLoad{T}(T,LocalLoadOption)"/>
        /// </summary>
        /// <param name="directoryName">読み込むディレクトリ名</param>
        /// <param name="fileName">読み込むファイル名</param>
        /// <param name="option">設定</param>
        /// <returns>読み込んでデシリアライズしたデータ、ファイルがなかったり読み込めなければdefault(T)</returns>
        [CanBeNull, Pure, PublicAPI]
        public static T Load<T>(
            [NotNull] string directoryName,
            [NotNull] string fileName,
            [CanBeNull] LocalLoadOption option = default)
            where T : ISerializable
        {
            if (string.IsNullOrEmpty(directoryName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(directoryName));
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(fileName));
            return Load<T>(GetFilePath(directoryName, fileName), option);
        }

        [CanBeNull, Pure]
        private static T Load<T>([NotNull] string filePath, [CanBeNull] LocalLoadOption option) where T : ISerializable
        {
            option ??= LocalLoadOption.Default;
            Log($"LocalLoad: '{filePath}'");
            if (!File.Exists(filePath))
            {
                Log("File not found.");
                return LoadOnFailure<T>(filePath, option);
            }

            try
            {
                return LoadWithoutRestoreFromBackup<T>(filePath, option) ?? LoadOnFailure<T>(filePath, option);
            }
            catch (Exception e)
            {
                LogException(e);
                return LoadOnFailure<T>(filePath, option);
            }
        }

        /// <summary>
        /// 読み込み失敗時の処理
        /// </summary>
        [CanBeNull, Pure]
        private static T LoadOnFailure<T>([NotNull] string filePath, [NotNull] LocalLoadOption option) where T : ISerializable
        {
            // 例外は上に流さない
            try
            {
                if (option.RestoreFromBackupOnFailure)
                    return LoadWithoutRestoreFromBackup<T>($"{filePath}{BackupFileExtension}", option);
            }
            catch (Exception e)
            {
                // バックアップからの読み込みに失敗
                LogException(e);
            }

            try
            {
                var defaultBasePath = DefaultBaseDirectoryPath;
                // リセットしないならdefault(null)を返す
                if (!option.ResetBaseDirectoryOnFailure || BaseDirectoryPath == defaultBasePath)
                    return default;

                // リセットして、今度はリセットしないように設定を変更してからロードし直し
                LocalLoadOption tempOption;
                try
                {
                    LogWarning($"Reset the {nameof(BaseDirectoryPath)}({BaseDirectoryPath}) to {defaultBasePath}");
                    ResetBaseDirectoryPath(defaultBasePath);
                    tempOption = option.DeepCopy() ?? new LocalLoadOption();
                }
                catch (Exception e)
                {
                    LogException(e);
                    tempOption = new LocalLoadOption();
                }

                tempOption.ResetBaseDirectoryOnFailure = false;
                // ReSharper disable once TailRecursiveCall
                return Load<T>(filePath, tempOption);
            }
            catch (Exception e)
            {
                LogException(e);
                return default;
            }
        }

        /// <summary>
        /// 実際に読み込む処理
        /// </summary>
        [CanBeNull, Pure]
        private static T LoadWithoutRestoreFromBackup<T>([NotNull] string filePath, [NotNull] LocalLoadOption option) where T : ISerializable
        {
            // 例外は上に流す
            var loadData = MessagePackSerializer.Deserialize<T>(File.ReadAllBytes(filePath));
            if (loadData != null)
                Log("LocalLoad Success!");
            return loadData;
        }
#nullable enable
    }
}
