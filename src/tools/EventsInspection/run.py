from reader import *
from filter import *

DUMP_FILE = "C:\\MW\BFM\\BFM3\\BurnForMoney\\src\\tools\\BurnForMoney.EventsDump\\bin\\Debug\\events_dump.json"

data = load_all_events(DUMP_FILE)
events = flt_mk_flat(data)
flt = flt_by_aggId(events, "068d691e-3dad-4eac-9a17-855b8c0435ee")
flt = flt_by_has_field(flt, "StartDate")
flt = flt_by_field(flt, "StartDate", lambda x: "2019-03" in x)
# flt = flt_by_field(flt, "ActivityType", lambda x: x == "Hike")

activities = agg_field(flt, "ActivityId")
flt_act = flt_by_field(flt_by_has_field(events, "ActivityId"), "ActivityId", lambda x: x in activities)

points = 0
print("Hike events", len(flt_act))
for e in flt_act:
    if e["$type"] == "BurnForMoney.Domain.Events.ActivityDeleted_V2, BurnForMoney.Infrastructure":
        points -= e["PreviousData"]["Points"]
        pass
    elif  e["$type"] == "BurnForMoney.Domain.Events.ActivityAdded, BurnForMoney.Infrastructure":
        points += e["Points"]
    else:
        raise Exception("Unsupported event")

print("Points", points)
# print(flt_act)
dump_results(flt_act, "./out.json")
