require 'nokogiri'

def parse_test_results(xml_file)
  doc = Nokogiri::XML(File.read(xml_file))
  puts "Parsing #{xml_file}..."

  test_results = { total: 0, passed: 0, failed: 0, skipped: 0, suites: {} }

  # テストスイートごとに結果を集計
  doc.xpath('//test-suite[@type="TestFixture"]').each do |suite|
    suite_name = suite['fullname']
    suite_total = suite['testcasecount'].to_i
    suite_passed = suite.xpath('.//test-case[@result="Passed"]').size
    suite_failed = suite.xpath('.//test-case[@result="Failed"]').size
    suite_skipped = suite.xpath('.//test-case[@result="Skipped"]').size
    suite_duration = suite['duration'].to_f.round(3)
    
    suite_result = { 
      total: suite_total, 
      passed: suite_passed, 
      failed: suite_failed, 
      skipped: suite_skipped, 
      duration: suite_duration, 
      test_cases: [] 
    }

    # テストケースごとに結果を追加
    suite.xpath('.//test-case').each do |test_case|
      test_name = test_case['name']
      result = test_case['result']
      duration = test_case['duration'].to_f.round(3)

      result_icon = case result
                    when 'Passed' then '✅'
                    when 'Failed' then '❌'
                    when 'Skipped' then '⏭️'
                    else ''
                    end

      suite_result[:test_cases] << { name: test_name, result: result, result_icon: result_icon, duration: duration }
    end

    test_results[:total] += suite_total
    test_results[:passed] += suite_passed
    test_results[:failed] += suite_failed
    test_results[:skipped] += suite_skipped
    test_results[:suites][suite_name] = suite_result
  end

  test_results
end

# XMLファイルの読み込み
folder_name = ARGV[0]
output_file = ARGV[1]
pr_number = ENV['PR_NUMBER']
pr_url = ENV['PR_URL']
pr_title = ENV['PR_TITLE']
pr_body = ENV['PR_BODY'] || "N/A"
pr_created_at = ENV['PR_CREATED_AT']
pr_head_ref = ENV['PR_HEAD_REF']
pr_base_ref = ENV['PR_BASE_REF']

editmode_results = parse_test_results("#{folder_name}/editmode-results.xml")
playmode_results = parse_test_results("#{folder_name}/playmode-results.xml")

# 全体の統計を計算
total_tests = editmode_results[:total] + playmode_results[:total]
total_passed = editmode_results[:passed] + playmode_results[:passed]
total_failed = editmode_results[:failed] + playmode_results[:failed]
total_skipped = editmode_results[:skipped] + playmode_results[:skipped]
pass_rate = (total_passed.to_f / total_tests * 100).round(2)

# Markdown形式のレポートの初期化
markdown_report = "# Test Results Summary for PR [##{pr_number} - #{pr_title}](#{pr_url})\n\n"
markdown_report << "<details>\n<summary>pull request details</summary>\n\n"
markdown_report << "#### ##{pr_number} origin/#{pr_base_ref} <- origin/#{pr_head_ref}\n"
markdown_report << "### #{pr_title}\n"
markdown_report << "#{pr_body}\n"
markdown_report << "Created At: #{pr_created_at}\n"
markdown_report << "</details>\n\n---\n"

editmode_all_passed = editmode_results[:passed] == editmode_results[:total]
playmode_all_passed = playmode_results[:passed] == playmode_results[:total]
# 0.01単位まで
all_suites = editmode_results[:suites].values.sum { |suite| suite[:duration] } + playmode_results[:suites].values.sum { |suite| suite[:duration] }
editmode_total_result = "#{editmode_all_passed ? '✅' : '❌'} editmode-results.xml - #{editmode_results[:passed]}/#{editmode_results[:total]} - Passed in #{editmode_results[:suites].values.sum { |suite| suite[:duration] }}s"
playmode_total_result = "#{playmode_all_passed ? '✅' : '❌'} playmode-results.xml - #{playmode_results[:passed]}/#{playmode_results[:total]} - Passed in #{playmode_results[:suites].values.sum { |suite| suite[:duration] }}s"
markdown_report << "## #{editmode_all_passed && playmode_all_passed ? '✅' : '❌'} Test Results - #{total_passed}/#{total_tests} - Passed in #{all_suites.round(4)}s\n"
markdown_report << "### #{editmode_total_result}\n"
markdown_report << "### #{playmode_total_result}\n\n---\n"

# Editmodeの結果をMarkdownに追加
markdown_report << "<details>\n<summary>#{editmode_total_result}</summary>\n\n"
editmode_results[:suites].each do |suite_name, suite_result|
  markdown_report << "  - #{suite_result[:passed] == suite_result[:total] ? '✅' : '❌'} #{suite_name} - #{suite_result[:passed]}/#{suite_result[:total]} - Passed in #{suite_result[:duration]}s\n"
  suite_result[:test_cases].each do |test_case|
    markdown_report << "    - #{test_case[:result_icon]} #{test_case[:name]} - #{test_case[:result]} in #{test_case[:duration]}s\n"
  end
end
markdown_report << "</details>\n\n<br>\n"

# Playmodeの結果をMarkdownに追加
markdown_report << "<details>\n<summary>#{playmode_total_result}</summary>\n\n"
playmode_results[:suites].each do |suite_name, suite_result|
  markdown_report << "  - #{suite_result[:passed] == suite_result[:total] ? '✅' : '❌'} #{suite_name} - #{suite_result[:passed]}/#{suite_result[:total]} - Passed in #{suite_result[:duration]}s\n"
  suite_result[:test_cases].each do |test_case|
    markdown_report << "    - #{test_case[:result_icon]} #{test_case[:name]} - #{test_case[:result]} in #{test_case[:duration]}s\n"
  end
end
markdown_report << "</details>\n\n---\n\n"
markdown_report << "> [!IMPORTANT]\n>\n"
markdown_report << "> Total Tests: **#{total_tests}**\n>\n"
markdown_report << "> Passed: **#{total_passed}**\n>\n"
markdown_report << "> Failed: **#{total_failed}**\n>\n"
markdown_report << "> Skipped: **#{total_skipped}**\n>\n"
markdown_report << "> Pass Rate: **#{pass_rate}%**\n---\n"

# Markdownファイルの保存
puts "Generating Markdown report..."
puts markdown_report
File.write(output_file, markdown_report)

puts "Markdown report generated: #{output_file}"