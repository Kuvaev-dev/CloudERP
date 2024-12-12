using DatabaseAccess;
using DatabaseAccess.Repositories;
using System.Web.Mvc;
using Unity.AspNet.Mvc;
using Unity;
using Domain.Services;
using Unity.Injection;
using Domain.Models;
using CloudERP.Models;

namespace CloudERP.Helpers
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // DbContext
            container.RegisterType<CloudDBEntities>(
                new InjectionFactory(c => new CloudDBEntities())
            );

            // Repos
            container.RegisterType<IAccountControlRepository, AccountControlRepository>();
            container.RegisterType<IAccountHeadRepository, AccountHeadRepository>();
            container.RegisterType<IAccountSubControlRepository, AccountSubControlRepository>();
            container.RegisterType<IUserTypeRepository, UserTypeRepository>();
            container.RegisterType<ICategoryRepository, CategoryRepository>();

            // Services
            container.RegisterType<IAccountControlService, AccountControlService>();
            container.RegisterType<IAccountHeadService, AccountHeadService>();
            container.RegisterType<IAccountSubControlService, AccountSubControlService>();
            container.RegisterType<IUserTypeService, UserTypeService>();
            container.RegisterType<ICategoryService, CategoryService>();

            // Mapping (Domain -> DB Layer)
            container.RegisterType<Domain.Mapping.Base.IMapper<UserType, tblUserType>, Domain.Mapping.UserTypeMapper>();
            container.RegisterType<Domain.Mapping.Base.IMapper<AccountControl, tblAccountControl>, Domain.Mapping.AccountControlMapper>();
            container.RegisterType<Domain.Mapping.Base.IMapper<AccountHead, tblAccountHead>, Domain.Mapping.AccountHeadMapper>();
            container.RegisterType<Domain.Mapping.Base.IMapper<AccountSubControl, tblAccountSubControl>, Domain.Mapping.AccountSubControlMapper>();
            container.RegisterType<Domain.Mapping.Base.IMapper<Category, tblCategory>, Domain.Mapping.CategoryMapper>();

            // Mapping (View -> Domain)
            container.RegisterType<CloudERP.Mapping.Base.IMapper<UserType, UserTypeMV>, CloudERP.Mapping.UserTypeMapper>();
            container.RegisterType<CloudERP.Mapping.Base.IMapper<AccountControl, AccountControlMV>, CloudERP.Mapping.AccountControlMapper>();
            container.RegisterType<CloudERP.Mapping.Base.IMapper<AccountHead, AccountHeadMV>, CloudERP.Mapping.AccountHeadMapper>();
            container.RegisterType<CloudERP.Mapping.Base.IMapper<AccountSubControl, AccountSubControlMV>, CloudERP.Mapping.AccountSubControlMapper>();
            container.RegisterType<CloudERP.Mapping.Base.IMapper<Category, CategoryMV>, CloudERP.Mapping.CategoryMapper>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}