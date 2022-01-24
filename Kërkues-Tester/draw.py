from matplotlib.pyplot import plot, title, xlabel, ylabel, show


def draw(points: [tuple]):
    points = sorted(points, key=lambda p: p[1])
    points = sorted(points, key=lambda p: p[0], reverse=True)
    n = int(4 * len(points) / 5)
    points_avg = []
    for i in range(n, len(points)):
        points_avg.append(average(points[i-n:i]))
    x, y = zip(*points_avg)
    plot(x, y)
    title("P/R")
    xlabel("Recobrado")
    ylabel("Precision")
    show()


def average(numbers: [tuple]):
    x = 0
    y = 0
    for n in numbers:
        x += n[0]
        y += n[1]
    return tuple((x / len(numbers), y / len(numbers)))
