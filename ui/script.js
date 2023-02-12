const API_BASE_URL = 'http://localhost:5039';

async function performSearch(pageToken) {
    event.preventDefault();
    toggleButtonLoading('search-button');
    const results = document.getElementById('search-results');
    results.innerHTML = '';

    try {
        const { nextPageToken, videos } = await fetchVideos(pageToken);

        renderVideos(results, videos, nextPageToken);
    } catch (error) {
        results.innerHTML = 'An error occured.';
    } finally {
        toggleButtonLoading('search-button');
    }
}

function renderVideos(resultsElement, videos, nextPageToken) {
    const nextButton = (nextPageToken) => {
        const button = document.createElement('button');
        button.innerHTML = 'Next';
        button.addEventListener('click', () => performSearch(nextPageToken));
        return button;
    }

    if (videos.length === 0) {
        resultsElement.innerHTML = 'No matching videos.';
        return;
    }

    for (const video of videos) {
        resultsElement.appendChild(videoComponent(video));
    }

    resultsElement.appendChild(nextButton(nextPageToken));
}

async function fetchVideos(pageToken) {
    const params = new URLSearchParams();

    params.append('q', document.getElementById('search-input').value);

    if (pageToken) {
        params.append('pageToken', pageToken);
    }

    const response = await fetch(`${API_BASE_URL}/video?${params.toString()}`);

    return await response.json();
}

function toggleButtonLoading(elementId) {
    const button = document.getElementById(elementId);
    const loading = button.getAttribute('aria-busy') === 'true';
    button.setAttribute('aria-busy', !loading);
    button.innerHTML = loading ? 'Search' : 'Loading...';
}

function videoComponent(video) {
    const titleElement = (video) => {
        const title = document.createElement('h4');
        title.innerHTML = video.title;
        return title;
    };

    const imageElement = (video) => {
        const linkElement = (videoId) => {
            const link = document.createElement('a');
            link.setAttribute('href', `https://www.youtube.com/watch?v=${videoId}`);
            link.setAttribute('target', '_blank');
            return link;
        }

        const img = document.createElement('img');
        img.src = video.thumbnailUrl;
        img.alt = video.title;
        img.width = 400;

        const link = linkElement(video.id);
        link.append(img);
        return link;
    };

    const descriptionElement = (video) => {
        const description = document.createElement('p');
        description.innerHTML = video.description;
        return description;
    };

    const tagsElement = (video) => {
        const tagContainer = document.createElement('div');
        video.tags.forEach(tag => {
            const tagElement = document.createElement('a');
            tagElement.innerHTML = tag;
            tagContainer.append(tagElement);
        });
        return tagContainer;
    };

    const card = document.createElement('article');
    card.append(titleElement(video));
    card.append(imageElement(video));
    card.append(descriptionElement(video));
    card.append(tagsElement(video));

    return card;
}
