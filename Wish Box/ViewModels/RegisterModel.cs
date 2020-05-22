using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Wish_Box.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указана дата рождения")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true, NullDisplayText = "01/01/0001")]
        public DateTime dayOfBirth { get; set; }

        [Required(ErrorMessage = "Не указана страна")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Не указан город")]
        public string City { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [MinLength(5,ErrorMessage = "Пароль должен содержать латинские заглавные и прописные буквы, а также цифры. Минимальное кол-во символов - 5"), 
            RegularExpression(@"^[a-zA-Z0-9'\s-]*$", ErrorMessage = "Пароль должен содержать латинские заглавные и прописные буквы, а также цифры. Минимальное кол-во символов - 5")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        public IFormFile Avatar { get; set; }
        public SelectList Countries { get; set; }
        public SelectList Cities { get; set; }

    }
}
