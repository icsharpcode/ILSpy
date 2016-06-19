import os, sys

def check(filename):
	ok = True
	with open(filename, 'r') as f:
		for i, line in enumerate(f):
			if line.startswith(' '):
				print('{}:{}: Line starting with spaces. Use tabs for indentation instead!'.format(filename, i+1))
				ok = False
	return ok

def main():
	root_dir = os.path.normpath(os.path.join(os.path.dirname(__file__), '..'))
	dirs_to_check = (
		os.path.join(root_dir, subdir)
		for subdir in ('ICSharpCode.Decompiler', 'ILSpy', 'ILSpy.BamlDecompiler'))
	ok = True
	for dir in dirs_to_check:
		for root, dirs, files in os.walk(dir):
			if '\\obj\\' in root:
				continue
			for filename in files:
				if filename.lower().endswith('.cs'):
					if not check(os.path.join(root, filename)):
						ok = False
	print('Tidy check: {}'.format('successful' if ok else 'failed'))
	return 0 if ok else 1

if __name__ == '__main__':
	sys.exit(main())