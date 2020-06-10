using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Wish_Box.ViewModels
{
    public class Edit1Model
    {
        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указана дата рождения")]
        [DataType(DataType.Date)]
        public DateTime dayOfBirth { get; set; }

        [Required(ErrorMessage = "Не указана страна")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Не указан город")]
        public string City { get; set; }
        
        public string Avatar { get; set; }
    }
}
