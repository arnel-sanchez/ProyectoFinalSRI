from os import getcwd
from os.path import join, pardir, abspath

from peek import queries, results
from test import test


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
    test(queries_list, queries_result)


if __name__ == '__main__':
    main()
