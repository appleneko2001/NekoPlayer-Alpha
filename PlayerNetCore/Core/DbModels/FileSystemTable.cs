using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Core.DbModels
{
    [Table(Name = "FileSystemTable")]
    public class FileSystemTable
    {
        [PrimaryKey, Identity]
        public long Id { get; set; }
        [Column, NotNull]
        public string ParentDir { get; set; } = "\\";
        [Column, NotNull]
        public bool IsFolder { get; set; } = false;
        [Column, NotNull]
        public string Name { get; set; }
        [Column]
        public string Data { get; set; }
    }
}
