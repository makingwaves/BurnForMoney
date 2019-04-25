from reader import *

def flt_mk_flat(data):
    flat_events = []
    aggs = get_aggregates(data)
    for agg in aggs:
        agg_events = get_events(agg, data)
        for e in agg_events:
            e["__agg"] = agg.lower()
        flat_events.extend(agg_events)

    return flat_events

def flt_by_aggId(flat_events, agg_id):
    agg_id = agg_id.lower()
    return [e for e in flat_events if e["__agg"] == agg_id]

def flt_by_has_field(flat_events, fld_name):
    return [e for e in flat_events if fld_name in e]

def flt_by_field(flat_events, fld_name, test):
    return [e for e in flat_events if test(e[fld_name])]

def get_field(flat_events, fld_name):
    return [e[fld_name] for e in flat_events]

def dist(flat_list):
    return list(set(flat_list))

def agg_field(flat_events, fld_name):
    return dist(get_field(flat_events, fld_name))