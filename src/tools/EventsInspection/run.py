from reader import *
from filter import *
import collections

DUMP_FILE = "C:\\MW\BFM\\BFM3\\BurnForMoney\\src\\tools\\BurnForMoney.EventsDump\\bin\\Debug\\events_dump.json"

data = load_all_events(DUMP_FILE)
events = flt_mk_flat(data)
flt = flt_by_has_field(events, "ExternalId")
external_ids = get_field(flt, "ExternalId")
external_ids_cns = [x for x in collections.Counter(external_ids).most_common() if x[1] > 1]
external_ids = [x[0] for x in external_ids_cns if x[0]]

flt = flt_by_has_field(events, "ExternalId")
flt = flt_by_field(flt, "ExternalId", lambda x: x in external_ids)
res_dict = {}

for f in flt:
    if f["ExternalId"] not in res_dict:
        res_dict[f["ExternalId"]] = []
    res_dict[f["ExternalId"]].append(f["StartDate"])

res_dict = [(k,v) for k,v in res_dict.items() if v[0].startswith("2019")]
print(res_dict)

# flt = flt_by_has_field(flt, "StartDate")
# flt = flt_by_field(flt, "StartDate", lambda x: "2019-03" in x)
# flt = flt_by_field(flt, "ActivityType", lambda x: x == "Hike")

# activities = agg_field(flt, "ActivityId")
# flt_act = flt_by_field(flt_by_has_field(events, "ActivityId"), "ActivityId", lambda x: x in activities)

# points = 0
# print("Hike events", len(flt_act))
# for e in flt_act:
#     if e["$type"] == "BurnForMoney.Domain.Events.ActivityDeleted_V2, BurnForMoney.Infrastructure":
#         points -= e["PreviousData"]["Points"]
#         pass
#     elif  e["$type"] == "BurnForMoney.Domain.Events.ActivityAdded, BurnForMoney.Infrastructure":
#         points += e["Points"]
#     else:
#         raise Exception("Unsupported event")

# print("Points", points)
# print(flt)
# dump_results(flt, "./out.json")
