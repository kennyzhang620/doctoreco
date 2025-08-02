import math
from fastapi import FastAPI, Request
import requests
from dotenv import load_dotenv
import os
from transformers import pipeline

app = FastAPI()
load_dotenv()

def get_route_from_google(origin_lat, origin_lon, destination_lat, destination_lon):
    api_key = os.environ.get("GOOGLE_MAPS_API_KEY")
   # print(origin_lat, origin_lon, destination_lat, destination_lon)
    if not api_key:
        raise RuntimeError("GOOGLE_MAPS_API_KEY environment variable not set")
    url = "https://routes.googleapis.com/directions/v2:computeRoutes"
    headers = {
        "Content-Type": "application/json",
        "X-Goog-Api-Key": api_key,
        "X-Goog-FieldMask": "routes.duration,routes.distanceMeters,routes.legs.steps"
    }
    data = {
        "origin": {"location": {"latLng": {"latitude": origin_lat, "longitude": origin_lon}}},
        "destination": {"location": {"latLng": {"latitude": destination_lat, "longitude": destination_lon}}},
        "travelMode": "WALK"
    }
    response = requests.post(url, headers=headers, json=data)
    response.raise_for_status()
    return response.json()


'''
curl --location 'https://api.zembra.io/listing/find?name=Eagle+Ridge+Hospital' \
--header 'Authorization: Bearer abcd' > out.json
'''

def get_results_from_zambra(query: str):
    api_key = os.environ.get("ZABRA_API_KEY")
   # print(origin_lat, origin_lon, destination_lat, destination_lon)
    if not api_key:
        raise RuntimeError("ZAMBRA_API_KEY environment variable not set")
    url = "https://api.zembra.io/listing/find?name=" + query
    headers = {
        "Content-Type": "application/json",
        "Authorization: Bearer": api_key,
    }
    response = requests.get(url, headers=headers)
    response.raise_for_status()
    return response.json()

# Equirectangular conversion function
def latlon_to_xy(origin, points):
    R = 6371000  # Earth radius in meters
    lat0 = math.radians(origin[0])
    lon0 = math.radians(origin[1])

    lat_rad = math.radians(points[0])
    lon_rad = math.radians(points[1])

    x = R * (lon_rad - lon0) * math.cos(lat0)
    y = R * (lat_rad - lat0)

    return (x, y)

def generate_localised_coords(origin,routes):
    localised_coords = {"coords": []}
    for r in routes:
        for l in r['legs']:
            for s in l['steps']:
                localised_coords["coords"].append( {"lat": latlon_to_xy(origin, [s['endLocation']['latLng']['latitude'], s['endLocation']['latLng']['longitude']])[0], "lon": latlon_to_xy(origin, [s['endLocation']['latLng']['latitude'], s['endLocation']['latLng']['longitude']])[1], "instruction": s['navigationInstruction']['instructions']})
    return localised_coords


test = {'routes': [{'legs': [{'steps': [{'distanceMeters': 243, 'staticDuration': '196s', 'polyline': {'encodedPolyline': 'qtekHlajnVkLS'}, 'startLocation': {'latLng': {'latitude': 49.1861691, 'longitude': -123.10055030000001}}, 'endLocation': {'latLng': {'latitude': 49.1883124, 'longitude': -123.10045190000001}}, 'navigationInstruction': {'maneuver': 'DEPART', 'instructions': 'Head north on Bearcroft Dr toward Daniels Rd'}, 'localizedValues': {'distance': {'text': '0.2 km'}, 'staticDuration': {'text': '3 mins'}}, 'travelMode': 'WALK'}, {'distanceMeters': 644, 'staticDuration': '533s', 'polyline': {'encodedPolyline': '}afkHx`jnV?kEGY?YFY@qDGY@]DWBiME[?oED]BsG?yE'}, 'startLocation': {'latLng': {'latitude': 49.1883124, 'longitude': -123.10045190000001}}, 'endLocation': {'latLng': {'latitude': 49.188265, 'longitude': -123.09166049999999}}, 'navigationInstruction': {'maneuver': 'TURN_RIGHT', 'instructions': 'Turn right onto Daniels Rd'}, 'localizedValues': {'distance': {'text': '0.6 km'}, 'staticDuration': {'text': '9 mins'}}, 'travelMode': 'WALK'}, {'distanceMeters': 403, 'staticDuration': '342s', 'polyline': {'encodedPolyline': 'safkHzihnVUC}EAEk@kIAa@CgB@e@D'}, 'startLocation': {'latLng': {'latitude': 49.188265, 'longitude': -123.09166049999999}}, 'endLocation': {'latLng': {'latitude': 49.1920486, 'longitude': -123.0914182}}, 'navigationInstruction': {'maneuver': 'TURN_LEFT', 'instructions': 'Turn left onto No 5 Rd'}, 'localizedValues': {'distance': {'text': '0.4 km'}, 'staticDuration': {'text': '6 mins'}}, 'travelMode': 'WALK'}, {'distanceMeters': 150, 'staticDuration': '135s', 'polyline': {'encodedPolyline': 'iyfkHjhhnV?yFBc@AqA@]'}, 'startLocation': {'latLng': {'latitude': 49.1920486, 'longitude': -123.0914182}}, 'endLocation': {'latLng': {'latitude': 49.192029999999995, 'longitude': -123.0894295}}, 'navigationInstruction': {'maneuver': 'TURN_RIGHT', 'instructions': 'Turn right onto Bridgeport Rd\nDestination will be on the left'}, 'localizedValues': {'distance': {'text': '0.2 km'}, 'staticDuration': {'text': '2 mins'}}, 'travelMode': 'WALK'}]}], 'distanceMeters': 1440, 'duration': '1205s'}]}

@app.get("/test/")
async def testd():
    return generate_localised_coords((49.1861691, -123.10055030000001), test['routes'])

@app.get("/queryzambra/")
async def get_zambras(query: str, skey: str):
    if skey != os.environ.get("SECRET_KEY"):
        return []

    v = get_results_from_zambra(query)
    if v:
        return v["data"]["agoda"].filter(lambda x: (query in x.name))
    else:
        return []

@app.get("/route/")
async def get_route(origin_lat: float, origin_lon: float, destination_lat: float, destination_lon: float, skey: str):
    if skey != os.environ.get("SECRET_KEY"):
        return []
    print(origin_lat, origin_lon, destination_lat, destination_lon)
    v = get_route_from_google(origin_lat, origin_lon, destination_lat, destination_lon)
    if v:
        return generate_localised_coords((origin_lat, origin_lon), v['routes'])
    else:
        return []
    
classifier = pipeline("sentiment-analysis")


@app.post("/predict")
async def predict(request: Request):
    data = await request.json()
    if not isinstance(data, list):
        return {"error": "Expected a list of JSON objects"}

    texts = [item.get("text") for item in data if "text" in item]
    if not texts:
        return {"error": "Expected JSON to have text field"}

    results = classifier(texts)
    return results