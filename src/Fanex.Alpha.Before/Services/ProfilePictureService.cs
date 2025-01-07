using Fanex.Alpha.Before.Criteria;
using Fanex.Alpha.Before.Helpers;
using Fanex.Alpha.Before.Models;
using Fanex.Data.Repository;

namespace Fanex.Alpha.Before.Services
{
    public interface IProfilePictureService
    {
        Task<ProfilePicture> GetProfilePicture(int custId);
    }

    public class ProfilePictureService : IProfilePictureService
    {
        private readonly IPHelper ipHelper;
        private readonly Logging.ILogger logger;
        private readonly IDynamicRepository dynamicRepository;

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
    }
}