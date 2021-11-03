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
                    if (pi.DeclaringType == typeof(OAuthOptions)
                     && pi.PropertyType == typeof(string)
                     && pi.Name == nameof(Options.OAuthOptions.RedirectUrl))
                    {
                        return "http://localhost:5000/oauth/";
                    }
                }

                return new NoSpecimen();
            }
        }
    }
}