
def queries(path: str) -> [str]:
    lines = read_lines(path)
    query = ""
    queries_list = []
    for i in range(len(lines)):
        if lines[i].startswith(".I"):
            query = append_query(queries_list, query)
        elif lines[i].startswith(".W"):
            continue
        else:
            query += lines[i].replace("\n", " ")
    query = append_query(queries_list, query)
    return queries_list


def read_lines(path: str) -> [[str]]:
    file = open(path, 'r')
    lines = file.readlines()
    file.close()
    return lines


def append_query(queries_list: [str], query: str) -> str:
    if len(query) > 0:
        query = query.strip()
        if query.endswith("."):
            query = query[:len(query) - 1].strip()
        queries_list.append(query)
    return ""


def results(path: str) -> [[int]]:
    lines = read_lines(path)
    count = 1
    current = []
    results_list = []
    for line in lines:
        text = line.replace("\n", "").split(" ")
        numbers = []
        for x in text:
            if len(x) < 1:
                continue
            try:
                numbers.append(int(x))
            except Exception:
                break
        if len(numbers) < 3:
            raise Exception
        if count == numbers[0]:
            current.append(numbers[1])
        elif count < numbers[0]:
            append_result(results_list, current)
            current.append(numbers[1])
            count = numbers[0]
        else:
            raise Exception
    append_result(results_list, current)
    return results_list


def append_result(results_list: [[int]], current: [int]):
    if len(current) < 1:
        return
    results_list.append(current.copy())
    current.clear()
