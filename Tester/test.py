from requests import post
from json import loads

from draw import draw


def test(queries_list: [str], queries_result: [{int}]):
    url = "https://localhost:7290/api/Search/test"
    no_data = 0
    no_rr = 0
    precision_avg = 0
    recall_avg = 0
    f1_measure_avg = 0
    fallout_avg = 0
    n = 10  #
    r_precision_n_avg = 0
    recall_n_avg = 0
    f1_measure_n_avg = 0
    fallout_n_avg = 0
    points = []
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
        print("Precision: {}".format(round(precision, 4)))
        precision_avg += precision
        print("Recobrado: {}".format(round(recall, 4)))
        recall_avg += recall
        if precision > 0 or recall > 0:
            points.append((recall, precision))
        if f1_measure is not None:
            print("Medida-F1: {}".format(round(f1_measure, 4)))
            f1_measure_avg += f1_measure
        print("Fallo:     {}".format(round(fallout, 4)))
        fallout_avg += fallout
        r_precision_n, recall_n, f1_measure_n, fallout_n = ranking(queries_result[i], data, n)
        print("Precision_{}: {}".format(n, round(r_precision_n, 4)))
        r_precision_n_avg += r_precision_n
        print("Recobrado_{}: {}".format(n, round(recall_n, 4)))
        recall_n_avg += recall_n
        if f1_measure_n is not None:
            print("Medida-F1_{}: {}".format(n, round(f1_measure_n, 4)))
            f1_measure_n_avg += f1_measure_n
        print("Fallo_{}:     {}".format(n, round(fallout_n, 4)))
        fallout_n_avg += fallout_n
    print("\n------------------------------------------------------------\n")
    print("Evaluacion General:")
    print("Ningun documento recuperado:           {} ({}%)".format(no_data, round(no_data / len(queries_list) * 100, 2)))
    print("Ningun documento relevante recuperado: {} ({}%)".format(no_rr, round(no_rr / len(queries_list) * 100, 2)))
    len_no_data = len(queries_result) - no_data
    if len_no_data < 1:
        return
    print("Precision (promedio): {}".format(round(precision_avg / len_no_data, 4)))
    print("Recobrado (promedio): {}".format(round(recall_avg / len_no_data, 4)))
    if len_no_data - no_rr > 0:
        print("Medida-F1 (promedio): {}".format(round(f1_measure_avg / (len_no_data - no_rr), 4)))
    print("Fallo     (promedio): {}".format(round(fallout_avg / len_no_data, 4)))
    print("Precision_{} (promedio): {}".format(n, round(r_precision_n_avg / len_no_data, 4)))
    print("Recobrado_{} (promedio): {}".format(n, round(recall_n_avg / len_no_data, 4)))
    if len_no_data - no_rr > 0:
        print("Medida-F1_{} (promedio): {}".format(n, round(f1_measure_n_avg / (len_no_data - no_rr), 4)))
    print("Fallo_{}     (promedio): {}\n".format(n, round(fallout_n_avg / len_no_data, 4)))
    draw(points)


def evaluation(query_result: [int], data: [int]) -> (int, int, int, int):
    set_result = set(query_result)
    set_data = set(data)
    rr = len(set.intersection(set_result, set_data))
    ri = len(set.difference(set_data, set_result))
    nr = len(set.difference(set_result, set_data))
    ni = 1400 - rr - ri - nr
    return rr, ri, nr, ni


def calculate(rr: int, ri: int, nr: int, ni: int) -> (float, float, float, float):
    precision = rr / (rr + ri)
    fallout = ri / (ri + ni)
    recall = rr / (rr + nr)
    if rr < 1:
        return precision, recall, None, fallout
    f1_measure = 2 * precision * recall / (precision + recall)
    return precision, recall, f1_measure, fallout


def ranking(query_result: [int], data: [int], n: int) -> (float, float, float, float):
    data_ = data[:n]
    rr_, ri_, nr_, ni_ = evaluation(query_result, data_)
    return calculate(rr_, ri_, nr_, ni_)
