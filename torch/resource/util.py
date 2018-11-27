import yaml

with open("resource/config.yaml") as f:
  config = yaml.safe_load(f)

def debug_print(*args):
	if config["DEBUG_MODE"] == True:
		print(*args)
