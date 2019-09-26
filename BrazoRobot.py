from matplotlib.widgets import Slider, RadioButtons
from mpl_toolkits.mplot3d import Axes3D
import matplotlib.pyplot as plt
import numpy as np
import csv

def main():
    robot = Dibujar_Robot()


class Dibujar_Robot():
    def __init__(self):
        '''Definicion de las variables'''
        with open('medidas.txt') as medidas:
            csvMed = csv.reader(medidas, delimiter=',')
            valores = np.array([], dtype=np.int32)
            for row in csvMed:
                valor = np.int32(row[1])
                valores = np.append(valores, valor)
        segmentos = int(1 + len(valores))  # número de segmentos
        self.l = np.array([0])  # Longitud del segmento
        for i in range(0,len(valores)):
            self.l = np.append(self.l, valores[i])
        self.l = np.array([0, 100, 100, 80])  # Longitud del segmento
        self.w = np.array([0] * segmentos, dtype=float)  # coordinadas horizontal
        self.z = np.array([0] * segmentos, dtype=float)  # coordinadas verticales
        self.x = np.array([0] * segmentos, dtype=float)  # componentes del eje x
        self.y = np.array([0] * segmentos, dtype=float)  # componentes del eje y
        self.a = np.array([np.pi] * segmentos, dtype=float)  # ángulo para el enlace
        #self.gripper_angle = np.array([0, -45, -90, 45, 90])  # angulos de la pinza preseleccionados
        self.gripper_angle = 0  # angulos de la pinza preseleccionados
        self.current_gripper = 2  # angulo de seleccion de la pinza
        self.tw = 30.0  # profundidad de posicion del eje w
        self.tz = 20.0  # altura de la posicion inicial en el eje z
        self.l12 = 0.0  # Hipotenusa entre a1 y a
        self.a12 = 0.0  # angulo inscrito entre la hipotenusa, w
        self.fig = plt.figure("Brazo Robot")  # Crear la ventana
        self.ax = plt.axes([0.05, 0.2, 0.90, .75], projection='3d')  # 3d ax panel
        self.axe = plt.axes([0.25, 0.85, 0.001, .001])  # panel para los mensajes de error

        '''Dibujar widgets'''
        # Dibujar el slider del panel a0
        axxval = plt.axes([0.35, 0.15, 0.45, 0.03])
        a0_val = Slider(axxval, 'rotacion a0', 0.0, 300, valinit=180)

        # Dibujar el slider del panel tw
        axyval = plt.axes([0.35, 0.1075, 0.45, 0.03])
        tw_val = Slider(axyval, 'extension w', 5, 280, valinit=20)

        # Dibujar el slider del panel xval
        axzval = plt.axes([0.35, 0.065, 0.45, 0.03])
        z_val = Slider(axzval, 'altura z', -50, 280, valinit=20)

        # Generar los radio buttons para la pinza
        rax = plt.axes([0.35, 0.0375, 0.45, 0.03])
        rax_val = Slider(rax, 'altura z', -90, 90, valinit=-90)

        self.display_error()  # Dibuja y oculta la ventana de error
        self.draw_robot()  # Funcion que dibuja el brazo robot

        #Modificar y redibujar el brazo cuando se modifica los paneles
        def update_a0_val(val):
            self.a[0] = np.deg2rad(val)
            self.draw_robot()

        a0_val.on_changed(update_a0_val)

        def update_tw_val(val):
            self.tw = val
            self.draw_robot()

        tw_val.on_changed(update_tw_val)

        def update_z_val(val):
            self.tz = val
            self.draw_robot()

        z_val.on_changed(update_z_val)


        #Definir los ángulos de la pinza (Cambiar por otro slider)
        def update_rax_val(val):
            self.gripper_angle = val
            self.draw_robot()


        rax_val.on_changed(update_rax_val)
        #set_gripper.on_clicked(set_gripper_angle)

        plt.show()

    '''Funciones'''

    def display_error(self):
        self.axe.set_visible(False)
        self.axe.set_yticks([])
        self.axe.set_xticks([])
        self.axe.set_navigate(False)
        self.axe.text(0, 0, 'Arm Can Not Reach the Target!', style='oblique',
                      bbox={'facecolor': 'red', 'alpha': 0.5, 'pad': 10}, size=20, va='baseline')

    def calc_p2(self):  # Calcula la posicion 2
        self.w[3] = self.tw
        self.z[3] = self.tz
        self.w[2] = self.tw - np.cos(np.radians(self.gripper_angle)) * self.l[3]
        self.z[2] = self.tz - np.sin(np.radians(self.gripper_angle)) * self.l[3]
        self.l12 = np.sqrt(np.square(self.w[2]) + np.square(self.z[2]))

    def calc_p1(self):  # Calcula la posicion 1
        self.a12 = np.arctan2(self.z[2], self.w[2])  # return the appropriate quadrant
        self.a[1] = np.arccos((np.square(self.l[1]) + np.square(self.l12) - np.square(self.l[2]))
                              / (2 * self.l[1] * self.l12)) + self.a12
        self.w[1] = np.cos(self.a[1]) * self.l[1]
        self.z[1] = np.sin(self.a[1]) * self.l[1]

    def calc_x_y(self):  # calcular x_y Pcoordinadas
        for i in range(len(self.x)):
            self.x[i] = self.w[i] * np.cos(self.a[0])
            self.y[i] = self.w[i] * np.sin(self.a[0])

    def set_positions(self):  # obtiene los valores x, y, z para la línea.
        # convierte los arreglos a líneas para dibujarlos
        xs = np.array(self.x).tolist()
        ys = np.array(self.y).tolist()
        zs = np.array(self.z).tolist()
        self.ax.cla()
        # Dibuja 2 líneas
        self.ax.plot(xs, ys, zs, 'o-', markersize=20,
                     markerfacecolor="orange", linewidth=8, color="blue")
        self.ax.plot(xs, ys, zs, 'o-', markersize=4,
                     markerfacecolor="blue", linewidth=1, color="silver")

    def set_ax(self):  # configuracion del panel ax
        self.ax.set_xlim3d(-200, 200)
        self.ax.set_ylim3d(-200, 200)
        self.ax.set_zlim3d(-5, 200)
        self.ax.set_xlabel('X axis')
        self.ax.set_ylabel('Y axis')
        self.ax.set_zlabel('Z axis')
        for j in self.ax.get_xticklabels() + self.ax.get_yticklabels():  # esconder ticks
            j.set_visible(False)
        self.ax.set_axisbelow(True)  # Enviar las líneas del grid para el fondo

    def get_angles(self):  # obtener todos los angulos
        self.a[2] = np.arctan((self.z[2] - self.z[1]) / (self.w[2] - self.w[1])) - self.a[1]
        self.a[3] = np.deg2rad(self.gripper_angle) - self.a[1] - self.a[2]
        angles = np.array(self.a).tolist()
        return angles

    def draw_robot(self):  # Dibuja y actualiza el modelo 3d del brazo
        self.calc_p2()
        self.calc_p1()
        self.calc_x_y()
        if self.l12 < (self.l[1] + self.l[2]):  # verifica los límites
            self.axe.set_visible(False)  # esconde el panel de error
            self.set_positions()
            self.set_ax()
            print
            np.around(np.rad2deg(self.get_angles()), 2)
        else:
            self.axe.set_visible(True)  # muestra el panel de rror
        plt.draw()


if __name__ == '__main__':
    main()
