using System;
using System.IO;
using JetBrains.Annotations;

#nullable enable

namespace Skyzi000.MessagePack.LocalSave
{
    public static partial class Local
    {
        /// <summary>
        /// ローカルセーブファイルを削除する
        /// </summary>
        /// <param name="data">削除するデータ(引き続き使える)</param>
        /// <param name="directoryName">削除するファイルのディレクトリ名(指定した場合は、<see cref="data"/>に設定されている<see cref="ILocalSaveData.DirectoryName"/>は無視される)</param>
        /// <param name="fileName">削除するファイル名(指定した場合は、<see cref="data"/>に設定されている<see cref="ILocalSaveData.FileName"/>は無視される)</param>
        /// <returns>成功ならtrue</returns>
        [PublicAPI]
        public static bool LocalDelete(this ILocalSaveData data, string? directoryName = null, string? fileName = null)
        {
            directoryName ??= data.DirectoryName;
            fileName ??= data.FileName;
            return DeleteFile(directoryName, fileName);
        }

        /// <inheritdoc cref="LocalDelete(ILocalSaveData,string?,string?)"/>
        [PublicAPI]
        public static bool Delete(ILocalSaveData data, string? directoryName = null, string? fileName = null)
            => data.LocalDelete(directoryName, fileName);

        /// <summary>
        /// ローカルセーブディレクトリを削除する
        /// </summary>
        /// <param name="directoryName">削除するディレクトリ名(nullなら<see cref="SavedBaseDirectoryPath"/>になる)</param>
        /// <param name="recursive">空でない場合、再帰的に削除するか</param>
        /// <param name="savedBaseDirectoryPath">保存されたベースディレクトリパスを参照する</param>
        /// <returns>成功ならtrue</returns>
        [PublicAPI]
        public static bool DeleteDirectory(string? directoryName, bool recursive, bool savedBaseDirectoryPath = true)
        {
            var baseDir = savedBaseDirectoryPath ? SavedBaseDirectoryPath : DefaultBaseDirectoryPath;
            var dirPath = directoryName == null ? baseDir : GetDirectoryPath(baseDir, directoryName);
            try
            {
                Directory.Delete(dirPath, recursive);
                return true;
            }
            catch (Exception e)
            {
                LogException(e);
                return false;
            }
        }

        /// <summary>
        /// ローカルセーブファイルを削除する
        /// </summary>
        /// <param name="directoryName">削除するディレクトリ名</param>
        /// <param name="fileName">削除するファイル名</param>
        /// <param name="savedBaseDirectoryPath">保存されたベースディレクトリパスを参照する</param>
        /// <returns>成功ならtrue</returns>
        [PublicAPI]
        public static bool DeleteFile(string directoryName, string fileName, bool savedBaseDirectoryPath = true)
        {
            var filePath = GetFilePath(savedBaseDirectoryPath ? SavedBaseDirectoryPath : DefaultBaseDirectoryPath, directoryName, fileName);
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (Exception e)
            {
                LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 全てのローカルセーブデータを削除する
        /// </summary>
        /// <returns>成功ならtrue</returns>
        [PublicAPI]
        public static bool DeleteAll() => DeleteDirectory(null, true);
    }
}
