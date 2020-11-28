namespace Davidlep.LINQPadDrivers.Common
{
    public class PropertyModel
    {
        public string RecordHeaderName { get; set; }
        public string CSharpType { get; set; }
        public TypeInferenceStates TypeInferenceState { get; set; } = TypeInferenceStates.Undetermied;
    }

    public enum TypeInferenceStates
    {
        Inferred,
        Undetermied,
        MultipleTypes
    }
}
