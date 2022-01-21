from requests import post
from json import loads
from os import getcwd
from os.path import join, pardir, abspath


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


def read_lines(path: str):
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


def results(path: str):
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


def post_all(queries_list: [str], queries_result: [{int}]):
    url = "https://localhost:7290/api/Search/test"
    no_data = 0
    no_rr = 0
    precision_avg = 0
    recall_avg = 0
    f1_measure_avg = 0
    fallout_avg = 0
    n = 10
    r_precision_n_avg = 0
    fallout_n_avg = 0
    for i in range(len(queries_list)):
        json = loads(post(url, json={"search": queries_list[i]}, verify=False).text)
        data = [int(x["name"]) for x in json["searchObjectResults"]]
        print("\nConsulta_{}: \"{}\"".format(i + 1, queries_list[i]))
        if len(data) < 1:
            print("-> No se recuperaron documentos")
            no_data += 1
            continue
        rr, ri, nr, ni = evaluation(queries_result[i], data)
        if rr < 1:
            print("-> No se recuperaron documentos relevantes")
            no_rr += 1
        precision, recall, f1_measure, fallout = calculate(rr, ri, nr, ni)
        print("Precision: {}".format(round(precision, 2)))
        precision_avg += precision
        print("Recobrado: {}".format(round(recall, 2)))
        recall_avg += recall
        if f1_measure is not None:
            print("Medida F1: {}".format(round(f1_measure, 2)))
            f1_measure_avg += f1_measure
        print("Fallo:     {}".format(round(fallout, 2)))
        fallout_avg += fallout
        r_precision_n, fallout_n = ranking(queries_result[i], data, n)
        print("R-Precision_{}: {}".format(n, round(r_precision_n, 2)))
        r_precision_n_avg += r_precision_n
        print("Fallo_{}:       {}".format(n, round(fallout_n, 2)))
        fallout_n_avg += fallout_n
    print("\n------------------------------------------------------------\n")
    print("Evaluacion General:")
    print("Ningun documento recuperado:           {} ({}%)".format(no_data, round(no_data / len(queries_list) * 100, 2)))
    print("Ningun documento relevante recuperado: {} ({}%)".format(no_rr, round(no_rr / len(queries_list) * 100, 2)))
    len_no_data = len(queries_result) - no_data
    if len_no_data < 1:
        return
    print("Precision (promedio):      {}".format(round(precision_avg / len_no_data, 2)))
    print("Recobrado (promedio):      {}".format(round(recall_avg / len_no_data, 2)))
    if len_no_data - no_rr > 0:
        print("Medida F1 (promedio):      {}".format(round(f1_measure_avg / (len_no_data - no_rr), 2)))
    print("Fallo (promedio):          {}".format(round(fallout_avg / len_no_data, 2)))
    print("R-Precision_{} (promedio): {}".format(n, round(r_precision_n_avg / len_no_data, 2)))
    print("Fallo_{} (promedio):       {}\n".format(n, round(fallout_n_avg / len_no_data, 2)))


def evaluation(query_result: [int], data: [int]) -> (int, int, int, int):
    set_result = set(query_result)
    set_data = set(data)
    rr = len(set.intersection(set_result, set_data))
    ri = len(set.difference(set_data, set_result))
    nr = len(set.difference(set_result, set_data))
    ni = 1400 - rr - ri - nr
    return rr, ri, nr, ni


def calculate(rr: int, ri: int, nr: int, ni: int, ranked: bool = False) -> (float, float, float, float):
    precision = rr / (rr + ri)
    fallout = ri / (ri + ni)
    if ranked:
        return precision, None, None, fallout
    recall = rr / (rr + nr)
    if rr < 1:
        return precision, recall, None, fallout
    f1_measure = 2 * precision * recall / (precision + recall)
    return precision, recall, f1_measure, fallout


def ranking(query_result: [int], data: [int], n: int):
    data_ = data[:n]
    rr_, ri_, nr_, ni_ = evaluation(query_result, data_)
    r_precision_n, _, _, fallout_n = calculate(rr_, ri_, nr_, ni_, True)
    return r_precision_n, fallout_n


def main():
    parent_path = abspath(join(getcwd(), pardir))
    path = parent_path + "\\Kërkues-Backend\\TestFiles\\"
    queries_path = path + "Cranfield\\cran.qry"
    results_path = path + "Cranfield\\cranqrel"
    # queries_path = path + "Medline\\MED.QRY"
    # results_path = path + "Medline\\MED.REL.OLD"
    # queries_path = path + "ADI\\ADI.QRY"
    # results_path = path + "ADI\\ADI.REL"
    queries_list = queries(queries_path)
    queries_result = results(results_path)
    post_all(queries_list, queries_result)


if __name__ == '__main__':
    main()
