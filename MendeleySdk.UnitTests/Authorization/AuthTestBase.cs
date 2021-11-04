using System.CodeDom;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using MendeleySdk.Options;
using Microsoft.Extensions.Options;

namespace MendeleySdk.UnitTests.Authorization
{
    public abstract class AuthTestBase
    {
        protected readonly IOptions<OAuthOptions> OAuthOptions;
        protected readonly Fixture Fixture;

        protected AuthTestBase()
        {
            Fixture = new();
            Fixture.Customizations.Add(new OAuthOptionGenerator());
            OAuthOptions = Microsoft.Extensions.Options.Options.Create(Fixture.Create<OAuthOptions>());
        }

        private class OAuthOptionGenerator : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (request is PropertyInfo pi)
                {
                    if (pi.DeclaringType == typeof(OAuthOptions))
                    {
                        if (pi.PropertyType == typeof(string))
                        {
                            switch (pi.Name)
                            {
                                case nameof(Options.OAuthOptions.RedirectUrl):
                                {
                                    return "http://localhost:10000/oauth/";
                                    break;
                                }
                                case nameof(Options.OAuthOptions.AuthBase):
                                {
                                    return "http://localhost:10001/oauth/authorize/";
                                    break;
                                }
                            }
                        }
                    }
                }

                return new NoSpecimen();
            }
        }
    }
}