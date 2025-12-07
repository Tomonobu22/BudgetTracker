// Callback that creates and populates a data table,
// instantiates the pie chart, passes in the data and
// draws it.
function drawChart(months, income, expenses, investments) {
    // Load the Visualization API and the corechart package.
    google.charts.load('current', { 'packages': ['line'] });

    // Set a callback to run when the Google Visualization API is loaded.
    google.charts.setOnLoadCallback(() => {
        // Create the data table.
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Month');
        data.addColumn('number', 'Income');
        data.addColumn('number', 'Expense');

        var rows = months.map((m, i) => [m, income[i], expenses[i]]);
        data.addRows(rows);

        // Set chart options
        var options = {
            chart: {
                title: `Monthly Cash Flow - ${year}`,
            },
            width: 900,
            height: 500,
            colors: ['#4CAF50', '#F44336'], // Green for income, red for expense
            backgroundColor: '#f8f9fa', // Light background
            chartArea: { width: '80%', height: '70%' },
            hAxis: {
                title: 'Month',
                titleTextStyle: { color: '#333', fontSize: 14, bold: true }
            },
            vAxis: {
                title: 'Amount ($)',
                titleTextStyle: { color: '#333', fontSize: 14, bold: true },
                gridlines: { color: '#e0e0e0' },
                minValue: 0
            },
            legend: {
                position: 'bottom',
                textStyle: { fontSize: 13 }
            }
        };

        // Instantiate and draw our chart, passing in some options.
        var chart = new google.charts.Line(document.getElementById('cash_flow_chart_div'));
        chart.draw(data, google.charts.Line.convertOptions(options));

        // Create additional table for income - expense difference
        var diffData = new google.visualization.DataTable();
        diffData.addColumn('string', 'Month');
        diffData.addColumn('number', 'Difference');

        rows = months.map((m, i) => [m, income[i] - expenses[i]]);
        diffData.addRows(rows);

        var diffOptions = {
            chart: {
                title: `Monthly Income - Expense Difference - ${year}`,
            },
            width: 900,
            height: 500,
            colors: ['#1E88E5'], // Blue tone for net income
            backgroundColor: '#f8f9fa', // Light background
            chartArea: { width: '80%', height: '70%' },
            hAxis: {
                title: 'Month',
                titleTextStyle: { color: '#333', fontSize: 14, bold: true }
            },
            vAxis: {
                title: 'Net Amount ($)',
                titleTextStyle: { color: '#333', fontSize: 14, bold: true },
                gridlines: { color: '#e0e0e0' },
                minValue: 0
            },
            legend: { position: 'none' }
        };
        // Instantiate and draw our chart, passing in some options.
        var diffChart = new google.charts.Line(document.getElementById('diff_chart_div'));
        diffChart.draw(diffData, google.charts.Line.convertOptions(diffOptions));

        // Create additional table for investments
        var invData = new google.visualization.DataTable();
        invData.addColumn('string', 'Month');
        invData.addColumn('number', 'Investments');

        var accumulatedInvestment = 0;
        rows = months.map((m, i) => {
            accumulatedInvestment += investments[i];
            return [m, accumulatedInvestment];
        });

        invData.addRows(rows);

        var invOptions = {
            chart: {
                title: `Investment Acc. - ${year}`,
            },
            width: 900,
            height: 500,
            colors: ['#26A69A'], // Teal/blue-green for investments
            backgroundColor: '#f8f9fa', // Light background
            chartArea: { width: '80%', height: '70%' },
            hAxis: {
                title: 'Month',
                titleTextStyle: { color: '#333', fontSize: 14, bold: true }
            },
            vAxis: {
                title: 'Investment Value ($)',
                titleTextStyle: { color: '#333', fontSize: 14, bold: true },
                gridlines: { color: '#e0e0e0' },
                minValue: 0
            },
            legend: { position: 'none' },
            lineWidth: 3,
            pointSize: 5
        };
        // Instantiate and draw our chart, passing in some options.
        var invChart = new google.charts.Line(document.getElementById('inv_chart_div'));
        invChart.draw(invData, google.charts.Line.convertOptions(invOptions));
    });
} 