namespace FileService.Api.Models
{
    public enum ErrorCode
    {
        success = 0,

        redirect = 1,
        invalid_params = 105,
        params_valid_fault = 106,

        unauthorized = 401,

        invalid_token = 402,
        invalid_username_or_password = 203,


        server_exception = -1000
    }
}
