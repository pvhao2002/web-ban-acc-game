using System.Collections.Generic;
using ProjectWeb.Models.Entity;

namespace ProjectWeb.Models.ViewModel
{
    public class ProfileViewModel
    {
        public Information information { get; set; } = new Information();
        public User u { get; set; } = new User();
        public ICollection<Order> listDoneOrder { get; set; } = new List<Order>();
        public ICollection<Order> listPendingOrder { get; set; } = new List<Order>();
        public ICollection<Order> listCancelOrder { get; set; } = new List<Order>();
    }

    public class Information
    {
        public string email { get; set; }
        public string fullName { get; set; }
    }
}