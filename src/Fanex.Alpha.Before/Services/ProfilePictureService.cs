using Alpha.Captcha.Core.Models;
using Fanex.Alpha.Before.Criteria;
using Fanex.Alpha.Before.Helpers;
using Fanex.Alpha.Before.Models;
using Fanex.Data.Repository;

namespace Fanex.Alpha.Before.Services
{
    public interface IProfilePictureService
    {
        Task<ProfilePicture> GetProfilePicture(string username);
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

        public async Task<ProfilePicture> GetProfilePicture(string username)
        {
            int custId = 0;
            if (custId > 0)
            {
                return await GetProfilePicture(custId);
            }

            return await GetRandomProfilePicture();
        }

        private async Task<ProfilePicture> GetRandomProfilePicture()
        {
            try
            {
                var profilePicture = await dynamicRepository.GetAsync<ProfilePicture>(new GetRandomProfilePictureCriteria());
                return profilePicture;
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);
                return new ProfilePicture();
            }
        }


        private async Task<ProfilePicture> GetProfilePicture(int custId)
        {
            try
            {
                var profilePicture = await dynamicRepository.GetAsync<ProfilePicture>(new GetProfilePictureCriteria(custId));
                if (profilePicture == null || string.IsNullOrEmpty(profilePicture.ImageBase64String))
                {
                    var result = await InsertDefaultProfilePicture(custId);
                    if (result.IsSuccess)
                    {
                        return await dynamicRepository.GetAsync<ProfilePicture>(new GetProfilePictureCriteria(custId));
                    }

                    return new ProfilePicture();
                }

                return profilePicture;
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);
                return new ProfilePicture();
            }
        }

        private async Task<Result> InsertDefaultProfilePicture(int custId)
        {
            try
            {
                var command = new UpdateProfilePictureCommand(custId, ipHelper.ClientIP());
                await dynamicRepository.ExecuteAsync(command);
                return new Result(command.ErrCode, command.ErrMsg);

            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);
                return new Result(100, "Auto insert Profile Picture Failed!");
            }
        }
    }
}