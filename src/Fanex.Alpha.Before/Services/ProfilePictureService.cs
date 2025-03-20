using Fanex.Alpha.Before.Criteria;
using Fanex.Alpha.Before.Helpers;
using Fanex.Alpha.Before.Models;
using Fanex.Data.Repository;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Fanex.Alpha.Before.Services
{
    public interface IProfilePictureService
    {
        Task<ProfilePicture> GetProfilePicture(int custId);

        public string MakeWaterMarkImage(byte[] imageBytes, string waterMarkText);
    }

    public class ProfilePictureService : IProfilePictureService
    {
        private readonly IPHelper ipHelper;
        private readonly Logging.ILogger logger;
        private readonly IDynamicRepository dynamicRepository;
        private readonly Random random = new Random();
        private readonly IEnumerable<Color> randomColors = new List<Color>
        {
            Color.Red,
            Color.Blue,
            Color.Purple,
            Color.Brown,
            Color.Black
        };

        public ProfilePictureService(IDynamicRepository dynamicRepository, Logging.ILogger logger, IPHelper ipHelper)
        {
            this.dynamicRepository = dynamicRepository;
            this.logger = logger;
            this.ipHelper = ipHelper;
        }

        public async Task<ProfilePicture> GetProfilePicture(int custId)
        {
            try
            {
                if (custId > 0)
                {
                    var criteria = new GetProfilePictureCriteria(custId);
                    var profilePicture = await GetProfilePicture(criteria);
                    if (profilePicture == null || string.IsNullOrEmpty(profilePicture.ImageBase64String))
                    {
                        return await InsertDefaultProfilePicture(custId);
                    }

                    return profilePicture;
                }
                else
                {
                    return await GetRandomProfilePicture();
                }
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);
                return new ProfilePicture();
            }
        }

        private async Task<ProfilePicture> GetProfilePicture(GetProfilePictureCriteria criteria)
        {
            var profilePicture = await dynamicRepository.GetAsync<ProfilePicture>(criteria);
            return profilePicture ?? new ProfilePicture();
        }

        private async Task<ProfilePicture> InsertDefaultProfilePicture(int custId)
        {
            var command = new UpdateProfilePictureCommand(custId, ipHelper.ClientIP());
            await dynamicRepository.ExecuteAsync(command);
            return command.ErrCode == 0 ? new ProfilePicture { Image = command.Image } : new ProfilePicture();
        }

        public async Task<ProfilePicture> GetRandomProfilePicture()
        {
            try
            {
                var profilePicture = await dynamicRepository.GetAsync<ProfilePicture>(new GetRandomProfilePictureCriteria());
                return profilePicture ?? new ProfilePicture();
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);
                return new ProfilePicture();
            }
        }

        public string MakeWaterMarkImage(byte[] imageBytes, string waterMarkText)
        {
            using var image = Image.Load<Rgba32>(imageBytes);
            var font = SystemFonts.CreateFont("Arial", 24, FontStyle.Bold);

            image.Mutate(x => ApplyScalingWaterMark(x, waterMarkText, font));

            using var ms = new MemoryStream();
            image.SaveAsJpeg(ms);

            return $"data:image/jpeg;base64,{Convert.ToBase64String(ms.ToArray())}";
        }

        private IImageProcessingContext ApplyScalingWaterMark(IImageProcessingContext processingContext,
            string text,
            Font font)
        {

            var x = 50;
            var y = 50;
            var textPosition = new PointF(x + random.Next(-20, 50), y + random.Next(-20, 50));

            var textSize = TextMeasurer.MeasureSize(text, new TextOptions(font));

            var backgroundRectangle = new Rectangle(
                (int)textPosition.X - 5,
                (int)textPosition.Y,
                (int)textSize.Width + 12,
                (int)textSize.Height + 10);

            processingContext.Fill(Color.LightBlue, backgroundRectangle);

            return processingContext.DrawText(text, font, randomColors.ElementAt(random.Next(0, 4)), textPosition);
        }
    }
}