#include "pch.h"
#include "GetLocation.h"

using namespace LocationComponent;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Devices::Geolocation;
using namespace Windows::Devices::Sensors;

GetLocation::GetLocation()
{

}

Windows::Foundation::Collections::IVector<double>^ GetLocation::ComputeResult() {
	auto res = ref new Vector<double>();
	res->Append(1);
	return res;
}

Windows::Foundation::IAsyncOperation<Windows::Devices::Geolocation::Geoposition ^>^ GetLocation::ReturnPosition() {
	
	auto geolocator = ref new Geolocator();
	auto pos = geolocator->GetGeopositionAsync();
	return pos;
}


