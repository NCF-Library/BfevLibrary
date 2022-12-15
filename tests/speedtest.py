from datetime import datetime
import evfl

start_time = datetime.now()

flow = evfl.EventFlow()
with open("100enemy.bfevfl", "rb") as file:
    flow.read(file.read())

with open("write_100enemy.bfevfl", "wb") as modified_file:
    flow.write(modified_file)

end_time = datetime.now()
# print(end_time.microsecond - start_time.microsecond)
# print(flow)
