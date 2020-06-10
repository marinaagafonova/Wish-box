using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Models
{
    public class Following
    {
        public int Id { get; set; }

        public int UserFId { get; set; } //кто подписан на пользователь
    //    public User UserF { get; set; }
        public int UserIsFId { get; set; } //на кого подписан пользователь
    //    public User UserIsF { get; set; }
    }
}
