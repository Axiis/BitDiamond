using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BitDiamond.Core.Services;
using Axis.Pollux.RBAC.Services;
using BitDiamond.Core.Services.Query;
using BitDiamond.Core.Services.Command;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using Axis.Luna.Extensions;
using Axis.Luna;
using Axis.Pollux.RBAC.Auth;
using System.Collections.Generic;
using System.Linq;

namespace BitDiamond.Test
{
    [TestClass]
    public class ReferalManagerTests
    {
        [TestMethod]
        public void AffixWithNoDownline()
        {
            IUserAuthorization authorizerMoq = new MockAuthorization();

            var userContextMoq = new Mock<IUserContext>();
            var apex = new User { UserId = "@apex", Status = AccountStatus.Active.As<int>() };
            userContextMoq.Setup(e => e.CurrentUser()).Returns(apex);


            var referalQueryMoq = new Mock<IReferralQuery>();
            referalQueryMoq.Setup(e => e.AllDownlines(It.IsAny<ReferalNode>()))
                           .Returns(new ReferalNode[0]);
            referalQueryMoq.Setup(e => e.GetReferalNode(It.IsAny<string>()))
                           .Returns((string rcode) => new ReferalNode
                           {
                               ReferenceCode = rcode,
                               ReferrerCode = null,
                               UplineCode = null,
                               User = apex
                           });
            referalQueryMoq.Setup(e => e.GetUserById(It.IsAny<string>()))
                           .Returns((string uid) => uid == "@apex" ? apex : new User
                           {
                               UserId = uid,
                               Status = AccountStatus.Active.As<int>()
                           });

            var pcommands = new Mock<IPersistenceCommands>();
            pcommands.Setup(e => e.Add(It.IsAny<ReferalNode>()))
                     .Returns((ReferalNode node) => Operation.Try(() => node));
            pcommands.Setup(e => e.Update(It.IsAny<ReferalNode>()))
                     .Returns((ReferalNode node) => Operation.Try(() => node));

            var refManager = new ReferralManager(userContextMoq.Object,
                                                authorizerMoq,
                                                referalQueryMoq.Object,
                                                pcommands.Object);

            var op = refManager.AffixNewUser("@random", "00000-00000-0000000000");

            Assert.IsNotNull(op);
            Assert.IsTrue(op.Succeeded);
        }

        [TestMethod]
        public void AffixWithDownline()
        {
            IUserAuthorization authorizerMoq = new MockAuthorization();

            var userContextMoq = new Mock<IUserContext>();
            var apex = new User { UserId = "@apex", Status = AccountStatus.Active.As<int>() };
            userContextMoq.Setup(e => e.CurrentUser()).Returns(apex);


            var referalQueryMoq = new Mock<IReferralQuery>();
            referalQueryMoq.Setup(e => e.AllDownlines(It.IsAny<ReferalNode>()))
                           .Returns(new ReferalNode[]
                           {
                               new ReferalNode
                               {
                                   ReferenceCode = "00000-00000-0000000001",
                                   ReferrerCode = "00000-00000-0000000000",
                                   UplineCode = "00000-00000-0000000000",
                                   User = new User
                                   {
                                       UserId = "@random1",
                                       Status = AccountStatus.Active.As<int>()
                                   }
                               },
                               new ReferalNode
                               {
                                   ReferenceCode = "00000-00000-0000000002",
                                   ReferrerCode = "00000-00000-0000000000",
                                   UplineCode = "00000-00000-0000000000",
                                   User = new User
                                   {
                                       UserId = "@random2",
                                       Status = AccountStatus.Active.As<int>()
                                   }
                               },
                               new ReferalNode
                               {
                                   ReferenceCode = "00000-00000-0000000003",
                                   ReferrerCode = "00000-00000-0000000000",
                                   UplineCode = "00000-00000-0000000002",
                                   User = new User
                                   {
                                       UserId = "@random3",
                                       Status = AccountStatus.Active.As<int>()
                                   }
                               },
                               new ReferalNode
                               {
                                   ReferenceCode = "00000-00000-0000000004",
                                   ReferrerCode = "00000-00000-0000000003",
                                   UplineCode = "00000-00000-0000000003",
                                   User = new User
                                   {
                                       UserId = "@random4",
                                       Status = AccountStatus.Active.As<int>()
                                   }
                               }
                           });
            referalQueryMoq.Setup(e => e.GetReferalNode(It.IsAny<string>()))
                           .Returns((string rcode) => new ReferalNode
                           {
                               ReferenceCode = rcode,
                               ReferrerCode = null,
                               UplineCode = null,
                               User = apex
                           });
            referalQueryMoq.Setup(e => e.GetUserById(It.IsAny<string>()))
                           .Returns((string uid) => uid == "@apex" ? apex : new User
                           {
                               UserId = uid,
                               Status = AccountStatus.Active.As<int>()
                           });

            var pcommands = new Mock<IPersistenceCommands>();
            pcommands.Setup(e => e.Add(It.IsAny<ReferalNode>()))
                     .Returns((ReferalNode node) => Operation.Try(() => node));
            pcommands.Setup(e => e.Update(It.IsAny<ReferalNode>()))
                     .Returns((ReferalNode node) => Operation.Try(() => node));

            var refManager = new ReferralManager(userContextMoq.Object,
                                                authorizerMoq,
                                                referalQueryMoq.Object,
                                                pcommands.Object);

            var op = refManager.AffixNewUser("@random", "00000-00000-0000000000");

            Assert.IsNotNull(op);
            Assert.IsTrue(op.Succeeded);
        }
    }

    public class MockAuthorization : IUserAuthorization
    {
        public Operation AddRole(string role) => Operation.NoOp();

        public Operation AssignRole(User user, string role) => Operation.NoOp();

        public Operation AuthorizeAccess(PermissionProfile authRequest, Action operation = null) => Operation.NoOp();

        public Operation AuthorizeAccess(PermissionProfile authRequest, Func<Operation> operation = null) => Operation.NoOp();

        public Operation<T> AuthorizeAccess<T>(PermissionProfile authRequest, Func<T> operation = null)
        => Operation.Try(() =>
        {
            if (operation != null) return operation.Invoke();
            else return default(T);
        });

        public Operation<T> AuthorizeAccess<T>(PermissionProfile authRequest, Func<Operation<T>> operation = null)
        {
            if (operation != null) return operation.Invoke();
            else return Operation.NoOp<T>();
        }

        public Operation<Role> CreateRole(string name) => Operation.NoOp<Role>();

        public Operation DeleteRole(Role role) => Operation.NoOp();

        public Operation DeleteUserRole(User user, Role role) => Operation.NoOp();

        public Dictionary<string, IEnumerable<Permission>> UserPermissions(User user) 
            => new Dictionary<string, IEnumerable<Permission>>();

        public IQueryable<Role> UserRoles(User user) => new Role[0].AsQueryable();
    }
}
