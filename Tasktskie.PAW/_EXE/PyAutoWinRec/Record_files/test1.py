# encoding: utf-8

import os, sys
script_dir = os.path.dirname(__file__)
sys.path.append(script_dir)
from pywinauto_recorder.player import *

send_keys("{VK_LWIN down}")
send_keys("{VK_LWIN up}")

with Window(u"Start||Window"):
	with Region(u"Search||Window"):
		left_click(u"||Edit%(-16.55,6.12)")
		send_keys("calculator""{ENTER}")

with Window(u"Calculator||Window"):
	with Region(u"Calculator||Window->||Group"):
		e1= left_click(u"Number pad||Group->One||Button")
		e1.draw_outline(colour='green', thickness=22)
		left_click(u"Standard operators||Group->Plus||Button%(-20.21,5.95)")
		e2 =left_click(u"Number pad||Group->Two||Button%(10.64,9.52)")
		e2.draw_outline(colour='green', thickness=22)
		left_click(u"Standard operators||Group->Equals||Button")
		e3 =left_click(u"Display is 3||Text%(39.10,-25.20)")
		e3.draw_outline(colour='green', thickness=22)

		# save result
		with open("C:/BlastAsia/Steeroid\Common/Temp/result.txt", "w") as f:  # Opens file and casts as f
			f.write("Hello World form " + f.name)  # Writing
		# File closed automatically
