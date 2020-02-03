using VoukoderManager.Core.Models;

namespace VoukoderManager.Core
{
    public static class VKEnumHelper
    {
        public static ProgramType GetMatchingType(ProgramType type)
        {
            switch (type)
            {
                case ProgramType.AfterEffects:
                    return ProgramType.VoukoderConnectorAfterEffects;

                case ProgramType.Premiere:
                case ProgramType.MediaEncoder:
                    return ProgramType.VoukoderConnectorPremiere;

                case ProgramType.VEGAS:
                    return ProgramType.VoukoderConnectorVegas;

                case ProgramType.VoukoderConnectorAfterEffects:
                    return ProgramType.AfterEffects;

                case ProgramType.VoukoderConnectorPremiere:
                    return ProgramType.Premiere;

                case ProgramType.VoukoderConnectorVegas:
                    return ProgramType.VEGAS;

                default:
                    return ProgramType.None;
            }
        }
    }
}