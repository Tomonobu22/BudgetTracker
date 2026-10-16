// Callback that creates and populates a data table,
// instantiates the pie chart, passes in the data and
// draws it.
function drawChart(months, income, expenses, investments, currentValues) {
    // Load the Visualization API and the corechart package.
    google.charts.load('current', { 'packages': ['corechart', 'line'] });

    // Set a callback to run when the Google Visualization API is loaded.
    google.charts.setOnLoadCallback(() => {
        // Create the data table.
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Month'); // x-label
        data.addColumn('number', 'Income');
        data.addColumn('number', 'Expense');

        data.addRows(months.map((m, i) => [m, income[i], expenses[i]]));

        // Set chart options
        var options = {
            title: `Monthly Cash Flow - ${year}`,
            width: 475,
            height: 265,
            colors: ['#4CAF50', '#F44336'],
            chartArea: { width: '80%', height: '70%' },
            vAxis: {
                title: 'Amount ($)',
                minValue: 0,
                gridlines: { color: '#e0e0e0' }
            },
            bar: { groupWidth: "60%" },
            legend: { position: 'bottom' }
        };

        // Instantiate and draw our chart, passing in some options.
        var chart = new google.visualization.ColumnChart(document.getElementById('cash_flow_chart_div'));
        chart.draw(data, options);

        // Create additional table for income - expense difference
        var diffData = new google.visualization.DataTable();
        diffData.addColumn('string', 'Month');
        diffData.addColumn('number', 'Difference');
        diffData.addColumn({ type: 'string', role: 'style' });

        for (var i = 0; i < months.length; i++) { 
            var diff = income[i] - expenses[i];
            var color = diff >= 0 ? '#4CAF50' : '#F44336';
            diffData.addRow([months[i], diff, color]);
        };

        var diffOptions = {
            title: `Monthly Income - Expense Difference - ${year}`,
            width: 475,
            height: 265,
            chartArea: { width: '80%', height: '70%' },
            hAxis: {
                title: 'Month',
                showTextEvery: 1,
            },
            vAxis: {
                title: 'Net Amount ($)',
                minValue: 0,
                gridlines: { color: '#e0e0e0' },
                minValue: Math.min(...income.map((v, i) => v - expenses[i])) - 500 // extend below if negative
            },
            legend: { position: 'none' },
            bar: { groupWidth: '60%' }
        };

        var diffChart = new google.visualization.ColumnChart(document.getElementById('diff_chart_div'));
        diffChart.draw(diffData, diffOptions);

        // Create additional table for investments vs current value
        // Expect `investments` to be contributions per month and `currentValues` to be
        // the market/value change for the portfolio (same length as months).
        // We will plot two series: cumulative Invested and cumulative Current Value
        var invData = new google.visualization.DataTable();
        invData.addColumn('string', 'Month');
        invData.addColumn('number', 'Invested');
        invData.addColumn('number', 'Current Value');
        // Tooltip column for combined info
        invData.addColumn({ type: 'string', role: 'tooltip' });

        var accumulatedInvested = 0;
        var accumulatedCurrent = 0;

        for (var i = 0; i < months.length; i++) {
            accumulatedInvested += (investments && investments[i]) ? investments[i] : 0;
            accumulatedCurrent += (currentValues && currentValues[i]) ? currentValues[i] : 0;

            // Calculate percent difference relative to invested (guard divide by zero)
            var pct = accumulatedInvested === 0 ? 0 : ((accumulatedCurrent - accumulatedInvested) / accumulatedInvested) * 100;
            var pctText = (pct >= 0 ? '+' : '') + pct.toFixed(1) + '%';

            var tooltip = months[i] + '\nInvested: $' + accumulatedInvested.toLocaleString() + '\nCurrent: $' + accumulatedCurrent.toLocaleString() + '\nChange: ' + pctText;

            invData.addRow([months[i], accumulatedInvested, accumulatedCurrent, tooltip]);
        }

        // Chart options
        var invOptions = {
            title: `Investment Accumulation - ${year}`,
            width: 500,
            height: 300,
            colors: ['#26A69A', '#2196F3'], // Teal for Invested, Blue for Current Value
            chartArea: { width: '80%', height: '70%' },
            hAxis: {
                title: 'Month',
                showTextEvery: 1,
            },
            vAxis: {
                title: 'Investment Value ($)',
                gridlines: { color: '#e0e0e0' },
                minValue: 0
            },
            legend: { position: 'bottom' },
            lineWidth: 3,
            pointSize: 5,
            tooltip: { isHtml: false }
        };

        // Classic LineChart with two series
        var invChart = new google.visualization.LineChart(document.getElementById('inv_chart_div'));
        invChart.draw(invData, invOptions);
    });
} 