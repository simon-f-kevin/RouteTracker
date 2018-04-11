#pragma once

namespace LocationComponent
{
    public ref class GetLocation sealed
    {
    public:
        GetLocation();
		Windows::Foundation::Collections::IVector<double>^ ComputeResult();
		Windows::Foundation::IAsyncOperation<Windows::Devices::Geolocation::Geoposition ^>^ ReturnPosition();
    };
}
