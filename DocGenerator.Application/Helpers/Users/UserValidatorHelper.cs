using DocGenerator.Application.DTOs.Users;
using System.Text.RegularExpressions;

namespace DocGenerator.Application.Helpers.Users
{
    public class UserValidatorHelper
    {
        /// <summary>
        /// Valida la creación del usuario
        /// </summary>
        public static List<string> ValidateCreate(CreateUserRequest request)
        {
            var errors = new List<string>();

            if (request == null)
            {
                errors.Add("Request inválido.");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(request.UserName))
                errors.Add("El nombre de usuario es obligatorio.");
            else
            {
                if (request.UserName.Length < 3)
                    errors.Add("El nombre de usuario debe tener al menos 3 caracteres.");

                if (!Regex.IsMatch(request.UserName, @"^[a-zA-Z0-9_.]+$"))
                    errors.Add("El nombre de usuario solo puede contener letras, números, _ y .");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("El correo electrónico es obligatorio.");
            else if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("El correo electrónico no tiene formato válido.");

            return errors;
        }
    }
}