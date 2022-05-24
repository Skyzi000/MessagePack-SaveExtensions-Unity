using System;
using System.IO;
using JetBrains.Annotations;

#nullable enable

namespace Skyzi000.MessagePack.LocalSave
{
    public static partial class Local
    {
        /// <summary>
        /// ローカルファイルに書き込む
        /// </summary>
        /// <param name="data">書き込むデータ</param>
        /// <param name="option">設定</param>
        /// <returns>成功ならtrue</returns>
        [PublicAPI]
        public static bool LocalSave(this ILocalSaveData data, LocalSaveOption? option)
            => data.LocalSave(null, null, option);

        /// <inheritdoc cref="LocalSave(ILocalSaveData,LocalSaveOption?)"/>
        [PublicAPI]
        public static bool Save(ILocalSaveData data, LocalSaveOption? option) => data.LocalSave(option);

        /// <summary>
        /// <inheritdoc cref="LocalSave(ILocalSaveData,LocalSaveOption?)"/>
        /// </summary>
        /// <param name="data">書き込むデータ</param>
        /// <param name="directoryName">書き込むディレクトリ名(指定した場合は、<see cref="data"/>に設定されている<see cref="ILocalSaveData.DirectoryName"/>は無視される)</param>
        /// <param name="fileName">書き込むファイル名(指定した場合は、<see cref="data"/>に設定されている<see cref="ILocalSaveData.FileName"/>は無視される)</param>
        /// <param name="option">設定</param>
        /// <returns><inheritdoc cref="LocalSave(ILocalSaveData,LocalSaveOption?)"/></returns>
        [PublicAPI]
        public static bool LocalSave(this ILocalSaveData data,
            string? directoryName = null,
            string? fileName = null,
            LocalSaveOption? option = default)
        {
            directoryName ??= data.DirectoryName;
            fileName ??= data.FileName;
            option ??= LocalSaveOption.Default;
            var filePath = GetFilePath(option.SaveSavedPathToPlayerPrefs ? SavedBaseDirectoryPath : DefaultBaseDirectoryPath, directoryName, fileName);
            Log($"LocalSave: '{filePath}'");
            var tempPath = $"{filePath}{TempFileExtension}";
            var tempFile = new FileInfo(tempPath);
            try
            {
                var bytes = data.Serialize();
                // 一時ファイルに書き込む
                DirectoryInfo? tempDir = tempFile.Directory;
                if (tempDir is { Exists: false })
                    CreateDirectory(tempDir.FullName, true);
                using (FileStream fs = tempFile.Create())
                {
                    fs.Write(bytes, 0, bytes.Length);
                    // 置き換え前検証
                    if (option.VerifyBeforeReplacement)
                    {
                        fs.Flush();
                        fs.Position = 0L;
                        var tempBytes = new byte[bytes.Length];
                        fs.Read(tempBytes, 0, tempBytes.Length);
                        fs.Close();
                        for (var i = 0; i < bytes.Length; i++)
                            if (bytes[i] != tempBytes[i])
                                throw new InvalidOperationException($"Failed to verify before replacement at index {i}");
                    }
                }

                if (!tempFile.Exists)
                    throw new InvalidOperationException("Failed to create temporary file");

                // 一時ファイルで置き換える
                if (File.Exists(filePath))
                    tempFile.Replace(filePath, option.BackupPreviousData ? $"{filePath}{BackupFileExtension}" : null, true);
                else
                    tempFile.MoveTo(filePath);
                // 置き換え後検証
                if (option.VerifyAfterReplacement)
                {
                    var fileBytes = File.ReadAllBytes(filePath);
                    if (bytes.Length != fileBytes.Length)
                        throw new InvalidOperationException("Failed to verify after replacement (Different Length)");
                    for (var i = 0; i < bytes.Length; i++)
                        if (bytes[i] != fileBytes[i])
                            throw new InvalidOperationException($"Failed to verify after replacement at index {i}");
                }

                Log("LocalSave Success!");
                return true;
            }
            catch (Exception e)
            {
                LogException(e);
                return false;
            }
        }

        /// <inheritdoc cref="LocalSave(ILocalSaveData,string?,string?,LocalSaveOption?)"/>
        [PublicAPI]
        public static bool Save(ILocalSaveData data,
            string? directoryName = null,
            string? fileName = null,
            LocalSaveOption? option = default)
            => data.LocalSave(directoryName, fileName, option);

        /// <summary>
        /// 指定されたパスにディレクトリがなければ作成する
        /// </summary>
        /// <param name="directoryPath">作成するディレクトリパス</param>
        /// <param name="recursive"><see cref="directoryPath"/>の親ディレクトリが存在しなければ再帰的に作成する</param>
        private static void CreateDirectory(string directoryPath, bool recursive)
        {
            var dirInfo = new DirectoryInfo(directoryPath);
            if (dirInfo.Exists)
                return;
            if (recursive)
            {
                DirectoryInfo? parent = dirInfo.Parent;
                if (parent is { Exists: false })
                    CreateDirectory(parent.FullName, recursive);
            }

            dirInfo.Create();
        }
    }
}
