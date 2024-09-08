require 'nokogiri'

def parse_test_results(xml_file)
  doc = Nokogiri::XML(File.read(xml_file))

  test_results = { total: 0, passed: 0, failed: 0, skipped: 0, test_cases: [] }

  # 各テストケースを解析して、結果を取得
  doc.xpath('//test-case').each do |test_case|
    suite_name = test_case.ancestors('test-suite').first['name']
    test_name = test_case['name']
    result = test_case['result']
    duration = test_case['duration']

    test_results[:total] += 1
    case result
    when 'Passed'
      test_results[:passed] += 1
      result_icon = '✅'
    when 'Failed'
      test_results[:failed] += 1
      result_icon = '❌'
    when 'Skipped'
      test_results[:skipped] += 1
      result_icon = '⏭️'
    else
      result_icon = ''
    end

    test_results[:test_cases] << { suite: suite_name, name: test_name, result: result, result_icon: result_icon, duration: duration }
  end

  test_results
end

# XMLファイルの読み込み
# フォルダ名を引数として受け取る
folder_name = ARGV[0]
editmode_results = parse_test_results(ARGV[0] + "/editmode-results.xml")
playmode_results = parse_test_results(ARGV[0] + "/playmode-results.xml")

# 全体の統計を計算
total_tests = editmode_results[:total] + playmode_results[:total]
total_passed = editmode_results[:passed] + playmode_results[:passed]
total_failed = editmode_results[:failed] + playmode_results[:failed]
total_skipped = editmode_results[:skipped] + playmode_results[:skipped]
pass_rate = (total_passed.to_f / total_tests * 100).round(2)

# Markdown形式のレポートの初期化
markdown_report = "# `Production` Test Results Summary\n\n"
markdown_report << "Total Tests: **#{total_tests}**\n"
markdown_report << "Passed: **#{total_passed}**\n"
markdown_report << "Failed: **#{total_failed}**\n"
markdown_report << "Skipped: **#{total_skipped}**\n"
markdown_report << "Pass Rate: **#{pass_rate}%**\n\n"

# Editmodeの結果をMarkdownに追加
markdown_report << "## Editmode Tests (#{editmode_results[:passed]}/#{editmode_results[:total]} Passed, #{(editmode_results[:passed].to_f / editmode_results[:total] * 100).round(2)}% Pass Rate)\n\n"
markdown_report << "| Result | Test Suite | Test Case | Duration |\n"
markdown_report << "|--------|------------|-----------|----------|\n"
editmode_results[:test_cases].each do |result|
  markdown_report << "| #{result[:result_icon]} | #{result[:suite]} | #{result[:name]} | #{result[:duration]} |\n"
end

markdown_report << "\n"

# Playmodeの結果をMarkdownに追加
markdown_report << "## Playmode Tests (#{playmode_results[:passed]}/#{playmode_results[:total]} Passed, #{(playmode_results[:passed].to_f / playmode_results[:total] * 100).round(2)}% Pass Rate)\n\n"
markdown_report << "| Result | Test Suite | Test Case | Duration |\n"
markdown_report << "|--------|------------|-----------|----------|\n"
playmode_results[:test_cases].each do |result|
  markdown_report << "| #{result[:result_icon]} | #{result[:suite]} | #{result[:name]} | #{result[:duration]} |\n"
end

# Markdownファイルの保存
File.write('Result.md', markdown_report)

puts "Markdown report generated: Result.md"