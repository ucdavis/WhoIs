using System;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace WhoIs.Models
{
    public class User : DomainObjectWithTypedId<Guid>
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string LoginId { get; set; }
    }

    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            ReadOnly();
            Table("MothraWhoIs");
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Email);
            Map(x => x.LoginId);
        }
    }
}