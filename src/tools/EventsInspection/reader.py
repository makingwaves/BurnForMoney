import json

def load_all_events(dump_file_path):
    with open(dump_file_path, 'r') as dump_file:
        return json.load(dump_file)


def get_aggregates(data):
    return list([agg for agg in data if agg != "$type"])
        
def get_events(agg_id, data):
    return data[agg_id]["$values"]

def validate_events_versions(events):
    prv_version = 0
    for e in events:
        if e["Version"] != prv_version + 1:
            raise Exception("e['Version'] != prv_version + 1. Event: {}".format(e))
        prv_version = e["Version"]

def dump_results(data, out_file_path):
        with open(out_file_path, "w") as out_file:
                json.dump(data, out_file)