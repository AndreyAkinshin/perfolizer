using Perfolizer.Metrology;

namespace Perfolizer.Exceptions;

public class InvalidMeasurementUnitExceptions(MeasurementUnit expected, MeasurementUnit actual)
    : InvalidOperationException($"Invalid measurement unit: expected {expected}, but was {actual}");