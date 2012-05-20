using System;
using FluentNHibernate.Mapping;
using Lucene.Net.Documents;
using UCDArch.Core.DomainModel;
using WhoIs.Services;

namespace WhoIs.Models
{
    public class User : DomainObjectWithTypedId<Guid>, ISearchDocument
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string LoginId { get; set; }
        
        public virtual Document GetDocument()
        {
            var document = new Document();
            document.Add(new Field("FirstName", FirstName, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("LastName", LastName, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Email", Email, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("LoginId", LoginId, Field.Store.YES, Field.Index.ANALYZED));
            //document.Add(new Field("Id", Id, Field.Store.NO, Field.Index.NO));
            return document;
        }
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