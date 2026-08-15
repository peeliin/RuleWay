import os

filepath = r"c:\Users\ASUS\Desktop\STAJ\RuleWay\RuleWay.UI\wwwroot\css\style.css"
with open(filepath, 'r', encoding='utf-8') as f:
    css = f.read()

# Backgrounds
css = css.replace('#f4f3f9', '#F8F9FA')
css = css.replace('#faf9fd', '#F8F9FA')
css = css.replace('#f8f7fc', '#F8F9FA')
css = css.replace('#f0eef5', '#F8F9FA')

# Borders
css = css.replace('#eae8f0', '#E5E7EB')
css = css.replace('#dddae6', '#E5E7EB')
css = css.replace('#f2f1f6', '#E5E7EB')
css = css.replace('#e5e2ed', '#E5E7EB')
css = css.replace('#e0dce8', '#E5E7EB')

# Texts
css = css.replace('#1e1e2d', '#202124')
css = css.replace('#8b87a0', '#6B7280')
css = css.replace('#5e5a6e', '#6B7280')
css = css.replace('#b5b0c8', '#6B7280')
css = css.replace('#7c789a', '#6B7280')

# Primary colors
css = css.replace('#7c5ce0', '#F4C430')
css = css.replace('#6a4fc9', '#D9AB1F')
css = css.replace('#a78bfa', '#F4C430') # Keep it simple

# Sidebar specifically
css = css.replace('background: linear-gradient(180deg, #2d2640 0%, #1a1528 100%);', 'background: #FFFFFF;\n    border-right: 1px solid #E5E7EB;')
css = css.replace('color: rgba(255,255,255,0.55);', 'color: #6B7280;')
css = css.replace('color: rgba(255,255,255,0.85);', 'color: #202124;')
css = css.replace('background: rgba(255,255,255,0.06);', 'background: #F8F9FA;')
css = css.replace('border-bottom: 1px solid rgba(255,255,255,0.06);', 'border-bottom: 1px solid #E5E7EB;')
css = css.replace('color: #ffffff;\n    letter-spacing: -0.3px;', 'color: #202124;\n    letter-spacing: -0.3px;') # .brand-text

# Nav item active states have color: #ffffff which we probably want to keep for yellow? Actually yellow background with white text might be hard to read. Yellow with dark text (#202124) is better.
css = css.replace('color: #ffffff;\n    box-shadow', 'color: #202124;\n    box-shadow')
css = css.replace('color: #ffffff;\n    border: none;', 'color: #202124;\n    border: none;')
css = css.replace('color: #fff;\n    border: none;', 'color: #202124;\n    border: none;')
css = css.replace('color: #fff;\n    border-radius: 9px;', 'color: #202124;\n    border-radius: 9px;')

# Hover colors for actions
css = css.replace('#eeebfb', '#FFF9E6') # edit button bg
css = css.replace('#e0dbf5', '#FDEBB3') # edit button hover bg
css = css.replace('#fbfafd', '#F8F9FA')

with open(filepath, 'w', encoding='utf-8') as f:
    f.write(css)

print("Color replacement successful.")
