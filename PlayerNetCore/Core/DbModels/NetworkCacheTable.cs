using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Core.DbModels
{
    [Table(Name = "CacheLinkTable")]
    public class CacheLinkTable
    {
        [PrimaryKey, Identity, NotNull]
        public long Id { get; set; }
        [Column, NotNull]
        public string Hash { get; set; }
    }
    [Table(Name = "LyricLinkTable")]
    public class LyricLinkTable
    {
        [PrimaryKey, Identity, NotNull]
        public long Id { get; set; }
        [Column, NotNull]
        public string Hash { get; set; }
        [Column, NotNull]
        public string ToFile { get; set; }
    }
    [Table(Name = "NetworkTagsTable")]
    public class NetworkTagsTable
    {
        [PrimaryKey, Identity]
        public long Id { get; set; }
        [Column, NotNull]
        public string Title { get; set; }
        [Column, NotNull]
        public string TagProvider { get; set; }
        [Column]
        public string Artist { get; set; }
        [Column]
        public string ArtistAlbum { get; set; }
        [Column]
        public string Album { get; set; }
        [Column]
        public string Year { get; set; }
        [Column]
        public string Genre { get; set; }
        [Column]
        public string AlbumImageLink { get; set; }
        [Column]
        public long AlbumImageId { get; set; }
    }
    [Table(Name = "ImageCacheTable")]
    public class ImageCacheTable
    {
        [PrimaryKey, Identity]
        public long Id { get; set; }
        [Column]
        public long PicId { get; set; }
        [Column, NotNull]
        public string AlbumImageLink { get; set; }
        [Column]
        public byte[] Data { get; set; }
    }
}
