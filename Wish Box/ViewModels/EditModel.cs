using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Wish_Box.ViewModels
{
    public class EditModel
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
    }
}
