import matplotlib.pyplot as plt

lambd = []
P0_theory = []
P0_real = []
Pn_theory = []
Pn_real = []
Q_theory = []
Q_real = []
A_theory = []
A_real = []
k_theory = []
k_real = []

with open('data.txt', 'r') as file:
    for line in file:
        data = line.replace(',','.').strip().split()
        if len(data) >= 11:
            lambd.append(float(data[0]))
            P0_theory.append(float(data[2]))
            P0_real.append(float(data[3]))
            Pn_theory.append(float(data[4]))
            Pn_real.append(float(data[5]))
            Q_theory.append(float(data[6]))
            Q_real.append(float(data[7]))
            A_theory.append(float(data[8]))
            A_real.append(float(data[9]))
            k_theory.append(float(data[10]))
            k_real.append(float(data[11]))

plt.plot(lambd, P0_theory, color='b', label='theoretical')
plt.plot(lambd, P0_real, color='r', label='realistic')
plt.xlabel("Intensity of demands received")
plt.ylabel("The probability of system downtime")
plt.title("Comparison of theoretical and real values of the probability of downtime")
plt.grid(True, linestyle='--', alpha=0.7)
plt.legend()
plt.savefig('result/p-1.png', dpi=300, bbox_inches='tight')
plt.close()

plt.plot(lambd, Pn_theory, color='b', label='theoretical')
plt.plot(lambd, Pn_real, color='r', label='realistic')
plt.xlabel("Intensity of demands received")
plt.ylabel("The probability of system failure")
plt.title("Comparison of theoretical and real values of the probability of failure")
plt.grid(True, linestyle='--', alpha=0.7)
plt.legend()
plt.savefig('result/p-2.png', dpi=300, bbox_inches='tight')
plt.close()

plt.plot(lambd, Q_theory, color='b', label='theoretical')
plt.plot(lambd, Q_real, color='r', label='realistic')
plt.xlabel("Intensity of demands received")
plt.ylabel("Relative throughput")
plt.title("Comparison of theoretical and real values of the relative throughput")
plt.grid(True, linestyle='--', alpha=0.7)
plt.legend()
plt.savefig('result/p-3.png', dpi=300, bbox_inches='tight')
plt.close()

plt.plot(lambd, A_theory, color='b', label='theoretical')
plt.plot(lambd, A_real, color='r', label='realistic')
plt.xlabel("Intensity of demands received")
plt.ylabel("Absolute throughput")
plt.title("Comparison of theoretical and real values of the absolute throughput")
plt.grid(True, linestyle='--', alpha=0.7)
plt.legend()
plt.savefig('result/p-4.png', dpi=300, bbox_inches='tight')
plt.close()

plt.plot(lambd, k_theory, color='b', label='theoretical')
plt.plot(lambd, k_real, color='r', label='realistic')
plt.xlabel("Intensity of demands received")
plt.ylabel("Average number of busy channels")
plt.title("Comparison of theoretical and real values of the average number of busy channels")
plt.grid(True, linestyle='--', alpha=0.7)
plt.legend()
plt.savefig('result/p-5.png', dpi=300, bbox_inches='tight')
plt.close()