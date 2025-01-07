namespace Fanex.Alpha.Before.Models
{
    public class ProfilePicture
    {
        public byte[] Image { get; init; }

        public string ImageBase64String => Image == null ? string.Empty : $"data:image/jpeg;base64,{Convert.ToBase64String(Image)}";
    }
}